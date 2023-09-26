using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MineCosmos.Bot.Entity;

namespace MineCosmos.Bot.Service;

public class CommandManagerService : BaseService, ICommandManagerService
{
    public CommandManagerService()
    {
    }

    /// <summary>
    /// 根据服务器ID查询指令组列表
    /// </summary>
    /// <param name="serverId"></param>
    /// <returns></returns>
    public async Task<List<CommandGroupEntity>> GetListCommandGroupByServerId(int serverId) => await GetListAsync<CommandGroupEntity>(a => a.ServerId == serverId);

    /// <summary>
    /// 根据主键ID获取指令组详情
    /// *导航查询子表
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<CommandGroupEntity> GetCommandGroupId(int id) =>
        await _db.Queryable<CommandGroupEntity>()
        .Includes(x => x.GroupItems)
        .FirstAsync();

    /// <summary>
    /// 保存指令组
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<bool> SaveCommandGroup(CommandGroupEntity model)
    {
        bool hasVal = await AnyAsync<CommandGroupEntity>(
            a => a.Name == model.Name && (model.Id < 0 || a.Id != model.Id));
        if (hasVal) throw new Exception("已存在相同名称的命令组");
        return await Save<CommandGroupEntity>(model);
      
    }

    /// <summary>
    /// 删除指令组
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<bool> RemoveCommandGroup(int id)
    {
        return await _db.DeleteNav<CommandGroupEntity>(x => x.Id == id)
              .Include(x => x.GroupItems)
              .ExecuteCommandAsync();
    }


    //TODO :  指令组子表的CRUD

}



