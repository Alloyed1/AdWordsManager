using AdWordsManager.Context.Contexts;
using AdWordsManager.Data.Base;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Linq;

namespace AdWordsManager.Providers.BaseProvider
{
    public abstract class BaseModelProvider<TDbModel> : IBaseModelProvider<TDbModel> where TDbModel : PocoBase 
    {
        public IQueryable<TDbModel> GetQueryable()
        {
            return new AdsDb().GetTable<TDbModel>();
        }

        public async Task<TDbModel> GetById(int id)
        {
            return await new AdsDb()
                            .GetTable<TDbModel>()
                            .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<TResult>> Select<TResult>(Expression<Func<TDbModel, bool>> predicate,
            Expression<Func<TDbModel, TResult>> selector)
        {
            return await new AdsDb()
                .GetTable<TDbModel>()
                .Where(predicate)
                .Select(selector)
                .ToArrayAsync();

        }

        public async Task<TDbModel> FirstOrDefault(Expression<Func<TDbModel, bool>> predicate)
        {
            return await new AdsDb()
                .GetTable<TDbModel>()
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<TDbModel>> GetAll() 
        {
            return await new AdsDb().GetTable<TDbModel>().ToArrayAsync();
        }
        public async Task<IEnumerable<TDbModel>> GetAll(Expression<Func<TDbModel, object>> predicate)
        {
            return await new AdsDb().GetTable<TDbModel>()
                .LoadWith(predicate)
                .ToArrayAsync();
        }
        public async Task Update(TDbModel model)
        {
           await new AdsDb().UpdateAsync(model);
        }
        public virtual async Task Create(TDbModel model)
        {
            await new AdsDb().InsertAsync(model);
        }
    }
}
