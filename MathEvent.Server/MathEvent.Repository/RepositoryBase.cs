using MathEvent.Contracts;
using MathEvent.Database;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MathEvent.Repository
{
    /// <summary>
    /// Базовый класс репозитория
    /// </summary>
    /// <typeparam name="T">Тип сущности, обрабатываемой в репозитории</typeparam>
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        /// <summary>
        /// Контекст данных для работы с базой данных
        /// </summary>
        protected ApplicationContext ApplicationContext { get; set; }

        public RepositoryBase(ApplicationContext applicationContext)
        {
            ApplicationContext = applicationContext;
        }

        public IQueryable<T> FindAll()
        {
            return ApplicationContext.Set<T>();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return ApplicationContext.Set<T>().Where(expression);
        }

        public async Task<T> CreateAsync(T entity)
        {
            var addResult = await ApplicationContext.Set<T>().AddAsync(entity);

            return addResult.Entity;
        }

        public void Update(T entity)
        {
            ApplicationContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            ApplicationContext.Set<T>().Remove(entity);
        }
    }
}
