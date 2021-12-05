using MathEvent.Contracts;
using MathEvent.Database;
using System;
using System.Linq;
using System.Linq.Expressions;

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

        public void Create(T entity)
        {
            ApplicationContext.Set<T>().Add(entity);
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
