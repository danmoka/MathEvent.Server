﻿using MathEvent.Contracts;
using MathEvent.Database;
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
        private ApplicationContext _applicationContext;

        /// <summary>
        /// Репозиторий для работы с Событиями
        /// </summary>
        private IEventRepository _event;

        /// <summary>
        /// Репозиторий для работы с Пользователями
        /// </summary>
        private IUserRepository _user;

        /// <summary>
        /// Репозиторий для работы с Подписками
        /// </summary>
        private ISubscriptionRepository _subscirption;

        /// <summary>
        /// Репозиторий для работы с Менеджерами
        /// </summary>
        private IManagementRepository _management;

        /// <summary>
        /// Репозиторий для работы с Файлами
        /// </summary>
        private IFileRepository _file;

        /// <summary>
        /// Репозиторий для работы с Владельцами
        /// </summary>
        private IOwnerRepository _owner;

        /// <summary>
        /// Репозиторий для работы с Организациями
        /// </summary>
        private IOrganizationRepository _organization;

        public RepositoryWrapper(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
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
                    _event = new EventRepository(_applicationContext);
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
                    _user = new UserRepository(_applicationContext);
                }

                return _user;
            }
        }

        /// <summary>
        /// Предоставляет репозиторий для работы с Подписками
        /// </summary>
        public ISubscriptionRepository Subscription
        {
            get
            {
                if (_subscirption is null)
                {
                    _subscirption = new SubscriptionRepository(_applicationContext);
                }

                return _subscirption;
            }
        }

        /// <summary>
        /// Предоставляет репозиторий для работы с Менеджерами
        /// </summary>
        public IManagementRepository Management
        {
            get
            {
                if (_management is null)
                {
                    _management = new ManagementRepository(_applicationContext);
                }

                return _management;
            }
        }

        /// <summary>
        /// Предоставляет репозиторий для работы с Файлами
        /// </summary>
        public IFileRepository File
        {
            get
            {
                if (_file is null)
                {
                    _file = new FileRepository(_applicationContext);
                }

                return _file;
            }
        }

        /// <summary>
        /// Предоставляет репозиторий для работы с Владельцами
        /// </summary>
        public IOwnerRepository Owner
        {
            get
            {
                if (_owner is null)
                {
                    _owner = new OwnerRepository(_applicationContext);
                }

                return _owner;
            }
        }

        /// <summary>
        /// Предоставляет репозиторий для работы с Организациями
        /// </summary>
        public IOrganizationRepository Organization
        {
            get
            {
                if (_organization is null)
                {
                    _organization = new OrganizationRepository(_applicationContext);
                }

                return _organization;
            }
        }

        /// <summary>
        /// Фиксирует изменения, совершенные репозиториями над контекстом данных
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            await _applicationContext.SaveChangesAsync();
        }
    }
}
