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
    public class BaseService: IBaseService
    {
        public BaseService() { }

        public SqlSugarScope _db = SqlSugarHelper.Instance;


        public async Task<bool> Save<TData>(TData model) where TData : class,new ()
            => await _db.Storageable(model).ExecuteCommandAsync() > 0;

        public async Task<bool> Remove<TData>(Expression<Func<TData, bool>>? whereExpression = null) 
            where TData : class,new ()
        {
            var has = await AnyAsync(whereExpression);
            if (!has) return false;

            return  await _db.Deleteable<TData>()
                .Where(whereExpression)
                .ExecuteCommandHasChangeAsync();
        }

        public async Task<bool> AnyAsync<TData>(Expression<Func<TData, bool>>? whereExpression)
      => await _db.Queryable<TData>().AnyAsync(whereExpression);

        public async Task<TData> GetAsync<TData>(Expression<Func<TData, bool>>? whereExpression)
     => await _db.Queryable<TData>().FirstAsync(whereExpression);

        public async Task<List<TData>> GetListAsync<TData>(Expression<Func<TData, bool>>? whereExpression)
  => await _db.Queryable<TData>().Where(whereExpression).ToListAsync();

        public async Task<ResPageDto<TData>> GetPageDataAsync<TData>(ReqPageDto model, Expression<Func<TData, bool>>? whereExpression = null, Expression<Func<TData, object>>? orderExpression = null, OrderByType type = OrderByType.Asc) where TData : class
        {
            RefAsync<int> total = 0;
            RefAsync<int> totalPage = 0;
            List<TData>? pageInfo = await _db.Queryable<TData>()
                .WhereIF(whereExpression!=null, whereExpression)
                .OrderByIF(orderExpression!=null, orderExpression,type)
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
    }

    public interface IBaseService
    {
        Task<bool> AnyAsync<TData>(Expression<Func<TData, bool>>? whereExpression);
        Task<TData> GetAsync<TData>(Expression<Func<TData, bool>>? whereExpression);
        Task<List<TData>> GetListAsync<TData>(Expression<Func<TData, bool>>? whereExpression);
        Task<ResPageDto<TData>> GetPageDataAsync<TData>(ReqPageDto model, Expression<Func<TData, bool>>? whereExpression = null, Expression<Func<TData, object>>? orderExpression = null, OrderByType type = OrderByType.Asc) where TData : class;
        Task<bool> Remove<TData>(Expression<Func<TData, bool>>? whereExpression = null) where TData : class, new();
        Task<bool> Save<TData>(TData model) where TData : class, new();
    }
}
