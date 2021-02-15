using MathEvent.Contracts;
using MathEvent.Entities;
using MathEvent.Entities.Models.Identities;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace MathEvent.Repository
{
    /// <summary>
    /// Оболочка для репозиториев. Позволяет не добавлять в DI все классы репозиториев
    /// </summary>
    public class RepositoryWrapper : IRepositoryWrapper
    {
        /// <summary>
        /// Контекст данных для работы с базой данных
        /// </summary>
        private RepositoryContext _repositoryContext;

        /// <summary>
        /// Менеджер для работы с пользователями
        /// </summary>
        private UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Репозиторий для работы с Событиями
        /// </summary>
        private IEventRepository _event;

        /// <summary>
        /// Репозиторий для работы с Пользователями
        /// </summary>
        private IUserRepository _user;

        public RepositoryWrapper(RepositoryContext repositoryContext, UserManager<ApplicationUser> userManager)
        {
            _repositoryContext = repositoryContext;
            _userManager = userManager;
        }

        /// <summary>
        /// Предоставляет репозиторий для работы с Событиями
        /// </summary>
        public IEventRepository Event
        {
            get
            {
                if (_event is null)
                {
                    _event = new EventRepository(_repositoryContext);
                }

                return _event;
            }
        }

        /// <summary>
        /// Предоставляет репозиторий для работы с Пользователями
        /// </summary>
        public IUserRepository User
        {
            get
            {
                if (_user is null)
                {
                    _user = new UserRepository(_repositoryContext, _userManager);
                }

                return _user;
            }
        }

        /// <summary>
        /// Фиксирует изменения, совершенные репозиториями над контекстом данных
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            await _repositoryContext.SaveChangesAsync();
        }
    }
}
