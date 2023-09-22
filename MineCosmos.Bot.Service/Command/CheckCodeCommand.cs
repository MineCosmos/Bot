using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MineCosmos.Bot.Entity;

namespace MineCosmos.Bot.Service.Command;
    public class CheckCodeCommand : BaseCommand, ICommand
    {
        private string code;
        public CheckCodeCommand(CommandData data)
        {
            _data = data;
            code = GetCommandArry()[0];
        }

        /// <summary>
        /// 暂时只处理绑定账号的验证码类型
        /// </summary>
        /// <returns></returns>
        public async Task<CommandExcuteResultModel> Execute()
        {
            var codeData = await GetCodeDataAsync(VerificationCodeEnum.绑定账号,code);
            if (codeData is null) return Error("该验证码无效或已过期");
         
            //数据库用户判断
            var dbuInfo = await Db.Queryable<PlayerInfoEntity>()
                .FirstAsync(a => a.Name == codeData.PlayerName);
            if (dbuInfo == null)
            {
                //新增
                dbuInfo = new PlayerInfoEntity()
                {
                    Name = codeData.PlayerName,
                    KookUserId = KookUserId.ToString()
                };
                await Db.Insertable(dbuInfo).ExecuteCommandAsync();
            }
            else
            {
                dbuInfo.KookUserId = KookUserId.ToString();

                //这个操作即可兼容玩家Minecraft玩家换号，所以玩家换号后需要重新绑定一次
                dbuInfo.Name = codeData.PlayerName;
                await Db.Updateable(dbuInfo).ExecuteCommandAsync();
            }

            return Success();
        }
    }

