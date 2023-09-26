using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MineCosmos.Bot.Entity.Dto;
using SqlSugar;

namespace MineCosmos.Bot.Service
{
    public class BaseService : IBaseService
    {
        public BaseService() { }

        #region 简单仓储

        /**
         * 不要封装任何直接写sql的，本项目没有企业开发中的历史业务包袱以及过于繁琐业务需求
         * （存储过程，视图 X 达咩）
         * - 当前采用CodeFirst模式，增加实体时，请务必参考sqlsugar官方文档某些特殊数据库的特殊字段类型特性
         * - 不要写类约束，可预见的未来很可能会调用一些第三方现成的功能，因此所建实体父类并不一定一致
         */

        public SqlSugarScope _db = SqlSugarHelper.Instance;

        public async Task<bool> Save<TData>(TData model) where TData : class, new()
            => await _db.Storageable(model).ExecuteCommandAsync() > 0;

        public async Task<bool> Remove<TData>(Expression<Func<TData, bool>>? whereExpression = null)
            where TData : class, new()
        {
            var has = await AnyAsync(whereExpression);
            if (!has) return false;

            return await _db.Deleteable<TData>()
                .Where(whereExpression)
                .ExecuteCommandHasChangeAsync();
        }

        public async Task<bool> AnyAsync<TData>(Expression<Func<TData, bool>>? whereExpression)
      => await _db.Queryable<TData>().AnyAsync(whereExpression);

        /// <summary>
        /// 修改指定列 SetColumns
        /// https://www.donet5.com/Home/Doc?typeId=1191
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="whereExpression">条件</param>
        /// <param name="columns">指定列：it => it.Num== it.Num+1 </param>
        /// <returns></returns>
        public async Task<bool> UpdateColumnsAsync<TData>(Expression<Func<TData, bool>>? whereExpression, Expression<Func<TData, bool>> columns) where TData : class, new()
        {
            return await _db.Updateable<TData>()
                 .SetColumns(columns)
                 .ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync<TData>(TData entity,Expression<Func<TData, bool>>? whereExpression) where TData : class, new()
        {
            return await _db.Updateable(entity)
                .Where(whereExpression)
                .ExecuteCommandHasChangeAsync();
        }

        public async Task<TData> GetAsync<TData>(Expression<Func<TData, bool>>? whereExpression)
     => await _db.Queryable<TData>().FirstAsync(whereExpression);

        public async Task<List<TData>> GetListAsync<TData>(Expression<Func<TData, bool>>? whereExpression)
  => await _db.Queryable<TData>().Where(whereExpression).ToListAsync();

        public async Task<ResPageDto<TData>> GetPageDataAsync<TData>(ReqPageDto model, 
            Expression<Func<TData, bool>>? whereExpression = null,           
            Expression<Func<TData, object>>? orderExpression = null, 
            OrderByType type = OrderByType.Asc) where TData : class
        {
            RefAsync<int> total = 0;
            RefAsync<int> totalPage = 0;
            List<TData>? pageInfo = await _db.Queryable<TData>()
                .WhereIF(whereExpression != null, whereExpression)
                .OrderByIF(orderExpression != null, orderExpression, type)
                .ToPageListAsync(model.PageNum, model.PageSize, total, totalPage);

            return new ResPageDto<TData>
            {
                Data = pageInfo,
                PageNum = model.PageNum,
                PageSize = model.PageSize,
                Total = total,
                TotalPage = totalPage
            };
        }
       
        /// <summary>
        ///  查询分页有导航
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="model"></param>
        /// <param name="include">导航查询</param>
        /// <param name="whereExpression"></param>
        /// <param name="orderExpression"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<ResPageDto<TData>> GetIncludesPageDataAsync<TData>(ReqPageDto model,
           Expression<Func<TData, bool>>? whereExpression = null,
           Expression<Func<TData, object>>? orderExpression = null,
           OrderByType type = OrderByType.Asc) where TData : class
        {
            // // Expression<Func<T, List<TReturn1>>> include1 = null,
            RefAsync<int> total = 0;
            RefAsync<int> totalPage = 0;
            List<TData>? pageInfo = await _db.Queryable<TData>()
                .IncludesAllFirstLayer()
                .WhereIF(whereExpression != null, whereExpression)
                .OrderByIF(orderExpression != null, orderExpression, type)
                .ToPageListAsync(model.PageNum, model.PageSize, total, totalPage);

            return new ResPageDto<TData>
            {
                Data = pageInfo,
                PageNum = model.PageNum,
                PageSize = model.PageSize,
                Total = total,
                TotalPage = totalPage
            };
        }

        #endregion
    }

    public interface IBaseService
    {
        /// <summary>
        ///  查询分页有导航
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="model"></param>
        /// <param name="whereExpression"></param>
        /// <param name="orderExpression"></param>
        /// <param name="type"></param>
        /// <returns></returns>
      Task<ResPageDto<TData>> GetIncludesPageDataAsync<TData>(ReqPageDto model,
           Expression<Func<TData, bool>>? whereExpression = null,
           Expression<Func<TData, object>>? orderExpression = null,
           OrderByType type = OrderByType.Asc) where TData : class;

        /// <summary>
        /// 修改指定列 SetColumns
        /// https://www.donet5.com/Home/Doc?typeId=1191
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="whereExpression">条件</param>
        /// <param name="columns">指定列：it => it.Num== it.Num+1 </param>
        /// <returns></returns>
        Task<bool> UpdateColumnsAsync<TData>(Expression<Func<TData, bool>>? whereExpression, Expression<Func<TData, bool>> columns) where TData : class, new();

        Task<bool> AnyAsync<TData>(Expression<Func<TData, bool>>? whereExpression);
        Task<TData> GetAsync<TData>(Expression<Func<TData, bool>>? whereExpression);
        Task<List<TData>> GetListAsync<TData>(Expression<Func<TData, bool>>? whereExpression);
        Task<ResPageDto<TData>> GetPageDataAsync<TData>(ReqPageDto model, Expression<Func<TData, bool>>? whereExpression = null, Expression<Func<TData, object>>? orderExpression = null, OrderByType type = OrderByType.Asc) where TData : class;
        Task<bool> Remove<TData>(Expression<Func<TData, bool>>? whereExpression = null) where TData : class, new();
        Task<bool> Save<TData>(TData model) where TData : class, new();
    }
}
