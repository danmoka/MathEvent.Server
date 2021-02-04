using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MathEvent.Contracts
{
    public interface IRepositoryBase<T>
    {
        IQueryable<T> FindAll();
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
        Task CreateAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
