using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AdWordsManager.Data.Base;
using System.Linq;
using System.Linq.Expressions;


namespace AdWordsManager.Providers.BaseProvider
{
    public interface IBaseModelProvider<TDbModel> where TDbModel : PocoBase
    {
        Task<IEnumerable<TDbModel>> GetAll();
        Task Update(TDbModel model);
        Task Create(TDbModel model);
        Task<TDbModel> GetById(int id);
        Task<IEnumerable<TResult>> Select<TResult>(Expression<Func<TDbModel, bool>> predicate, 
            Expression<Func<TDbModel, TResult>> selector);

        Task<TDbModel> FirstOrDefault(Expression<Func<TDbModel, bool>> predicate);

        IQueryable<TDbModel> GetQueryable();
    }
}
