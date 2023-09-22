using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MineCosmos.Bot.Common;
using MineCosmos.Bot.Entity.Base;
using MineCosmos.Bot.Service;
using SqlSugar;

namespace MineCosmos.Bot.Service
{
    public class SqlSugarHelper
    {
        private static readonly SqlSugarScope _instance;

        static SqlSugarHelper()
        {
            Console.WriteLine($"环境：{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");

            var lstDbConfigs = AppSettings.app<SqlSugarConfig>(new string[] { "ConnectionStrings", "SqlSugarConfig" });

            if (lstDbConfigs is null)
                throw new Exception("数据库没配置喔");

            Console.WriteLine($"数据库配置数量：{lstDbConfigs.Count}");

            var lstConfig = lstDbConfigs?.Adapt<List<ConnectionConfig>>();

            _instance = new SqlSugarScope(lstConfig, db =>
            {
                lstDbConfigs?.ForEach(it =>
                {
                    var sqlsugarScope = db.GetConnectionScope(it.ConfigId);//获取当前库
                    MoreSetting(sqlsugarScope);//更多设置
                    ExternalServicesSetting(sqlsugarScope, it);//实体拓展配置
                    AopSetting(sqlsugarScope);//aop配置
                    FilterSetting(sqlsugarScope);//过滤器配置
                });
            });
        }

        public static SqlSugarScope Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// 过滤器设置
        /// </summary>
        /// <param name="db"></param>
        public static void FilterSetting(SqlSugarScopeProvider db)
        {

        }

        /// <summary>
        /// 实体更多配置
        /// </summary>
        /// <param name="db"></param>
        private static void MoreSetting(SqlSugarScopeProvider db)
        {
            db.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings
            {
                SqlServerCodeFirstNvarchar = true,//设置默认nvarchar            
            };
        }

        /// <summary>
        /// 实体拓展配置,自定义类型多库兼容
        /// </summary>
        /// <param name="db"></param>
        /// <param name="config"></param>
        private static void ExternalServicesSetting(SqlSugarScopeProvider db, SqlSugarConfig config)
        {
            db.CurrentConnectionConfig.ConfigureExternalServices = new ConfigureExternalServices
            {
                // 处理表
                EntityNameService = (type, entity) =>
                {
                    if (config.IsUnderLine && !entity.DbTableName.Contains('_'))
                        entity.DbTableName = UtilMethods.ToUnderLine(entity.DbTableName); // 驼峰转下划线
                },
                //自定义类型多库兼容
                EntityService = (c, p) =>
                {
                    //如果是mysql并且是varchar(max) 已弃用
                    //if (config.DbType == SqlSugar.DbType.MySql && (p.DataType == SqlsugarConst.NVarCharMax))
                    //{
                    //    p.DataType = SqlsugarConst.LongText;//转成mysql的longtext
                    //}
                    //else if (config.DbType == SqlSugar.DbType.Sqlite && (p.DataType == SqlsugarConst.NVarCharMax))
                    //{
                    //    p.DataType = SqlsugarConst.Text;//转成sqlite的text
                    //}
                    //默认不写IsNullable为非必填
                    if (new NullabilityInfoContext().Create(c).WriteState is NullabilityState.Nullable)
                        p.IsNullable = true;
                    if (config.IsUnderLine && !p.IsIgnore && !p.DbColumnName.Contains('_'))
                        p.DbColumnName = UtilMethods.ToUnderLine(p.DbColumnName); // 驼峰转下划线

                }
            };



        }


        /// <summary>
        /// Aop设置
        /// </summary>
        /// <param name="db"></param>
        public static void AopSetting(SqlSugarScopeProvider db)
        {

            var config = db.CurrentConnectionConfig;

            // 设置超时时间
            db.Ado.CommandTimeOut = 30;
            // 打印SQL语句
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                //如果不是开发环境就打印sql
                //if (hostingContext.HostingEnvironment.IsDevelopment())
                //{
                if (sql.StartsWith("SELECT"))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    WriteSqlLog($"查询{config.ConfigId}库操作");
                }
                if (sql.StartsWith("UPDATE") || sql.StartsWith("INSERT"))
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    WriteSqlLog($"修改{config.ConfigId}库操作");
                }
                if (sql.StartsWith("DELETE"))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    WriteSqlLog($"删除{config.ConfigId}库操作");
                }
                Console.WriteLine(UtilMethods.GetSqlString(config.DbType, sql, pars));
                WriteSqlLog($"{config.ConfigId}库操作结束");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
                //}
            };
            //异常
            db.Aop.OnError = (ex) =>
            {
                //如果不是开发环境就打印日志
                //if (hostingContext.HostingEnvironment.IsDevelopment())
                //{
                if (ex.Parametres == null) return;
                Console.ForegroundColor = ConsoleColor.Red;
                var pars = db.Utilities.SerializeObject(((SugarParameter[])ex.Parametres).ToDictionary(it => it.ParameterName, it => it.Value));
                WriteSqlLog($"{config.ConfigId}库操作异常");
                Console.WriteLine(UtilMethods.GetSqlString(config.DbType, ex.Sql, (SugarParameter[])ex.Parametres) + "\r\n");
                Console.ForegroundColor = ConsoleColor.White;
                // }
            };
            //插入和更新过滤器
            db.Aop.DataExecuting = (oldValue, entityInfo) =>
            {
                // 新增操作
                if (entityInfo.OperationType == DataFilterType.InsertByObject)
                {

                    if (entityInfo.PropertyName == nameof(BaseEntity.CreateTime))
                        entityInfo.SetValue(DateTime.Now.ToString("yyyy-MM-dd"));

                    ////手机号和密码自动加密
                    //if (entityInfo.EntityName == nameof(SysUser) && (entityInfo.PropertyName == nameof(SysUser.Password) || entityInfo.PropertyName == nameof(SysUser.Phone)))
                    //    entityInfo.SetValue(CryptogramUtil.Sm4Encrypt(oldValue?.ToString()));
                    //if (App.User != null)
                    //{
                    //    //创建人和创建机构ID
                    //    if (entityInfo.PropertyName == nameof(BaseEntity.CreateUserId))
                    //        entityInfo.SetValue(App.User.FindFirst(ClaimConst.UserId)?.Value);
                    //    if (entityInfo.PropertyName == nameof(BaseEntity.CreateUser))
                    //        entityInfo.SetValue(App.User?.FindFirst(ClaimConst.Account)?.Value);
                    //    if (entityInfo.PropertyName == nameof(DataEntityBase.CreateOrgId))
                    //        entityInfo.SetValue(App.User.FindFirst(ClaimConst.OrgId)?.Value);
                    //}
                }
                // 更新操作
                if (entityInfo.OperationType == DataFilterType.UpdateByObject)
                {
                    //这里不能自动加密，不然redis数据会有问题
                    //if (entityInfo.PropertyName == nameof(SysUser.Password) || entityInfo.PropertyName == nameof(SysUser.Phone))
                    //    entityInfo.SetValue(CryptogramUtil.Sm4Encrypt(oldValue?.ToString()));
                    //更新时间
                    if (entityInfo.PropertyName == nameof(BaseEntity.UpdateTime))
                        entityInfo.SetValue(DateTime.Now);
                    ////更新人
                    //if (App.User != null)
                    //{
                    //    if (entityInfo.PropertyName == nameof(BaseEntity.UpdateUserId))
                    //        entityInfo.SetValue(App.User?.FindFirst(ClaimConst.UserId)?.Value);
                    //    if (entityInfo.PropertyName == nameof(BaseEntity.UpdateUser))
                    //        entityInfo.SetValue(App.User?.FindFirst(ClaimConst.Account)?.Value);
                    //}

                }
            };


            //查询数据转换 
            //db.Aop.DataExecuted = (value, entity) =>
            //{
            //    if (entity.Entity.Type == typeof(SysUser))
            //    {
            //        //如果手机号不为空
            //        if (entity.GetValue(nameof(SysUser.Phone)) != null)
            //        {
            //            //手机号数据转换
            //            var phone = CryptogramUtil.Sm4Decrypt(entity.GetValue(nameof(SysUser.Phone)).ToString());
            //            entity.SetValue(nameof(SysUser.Phone), phone);
            //        }
            //        //如果密码不为空
            //        if (entity.GetValue(nameof(SysUser.Password)) != null)
            //        {
            //            //密码数据转换
            //            var passwd = CryptogramUtil.Sm4Decrypt(entity.GetValue(nameof(SysUser.Password)).ToString());
            //            entity.SetValue(nameof(SysUser.Password), passwd);
            //        }

            //    }
            //};
        }


        private static void WriteSqlLog(string msg)
        {
            Console.WriteLine($"=============={msg}==============");
        }
    }
}
