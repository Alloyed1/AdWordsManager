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
            return new DataContext().GetTable<TDbModel>();
        }

        public async Task<TDbModel> GetById(int id)
        {
            return await new DataContext()
                            .GetTable<TDbModel>()
                            .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<TResult>> Select<TResult>(Expression<Func<TDbModel, bool>> predicate,
            Expression<Func<TDbModel, TResult>> selector)
        {
            return await new DataContext()
                .GetTable<TDbModel>()
                .Where(predicate)
                .Select(selector)
                .ToListAsync();

        }

        public async Task<TDbModel> FirstOrDefault(Expression<Func<TDbModel, bool>> predicate)
        {
            return await new DataContext()
                .GetTable<TDbModel>()
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<List<TDbModel>> GetAll() 
        {
            return await new DataContext().GetTable<TDbModel>().ToListAsync();
        }
        public async Task Update(TDbModel model)
        {
           await new DataContext().UpdateAsync(model);
        }
        public virtual async Task Create(TDbModel model)
        {
            await new DataContext().InsertAsync(model);
        }
    }
}
