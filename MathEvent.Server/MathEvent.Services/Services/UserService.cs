using AutoMapper;
using MathEvent.Contracts;
using MathEvent.Contracts.Services;
using MathEvent.DTOs.Users;
using MathEvent.Entities.Entities;
using MathEvent.Models.Others;
using MathEvent.Models.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MathEvent.Services.Services
{
    /// <summary>
    /// Сервис по выполнению действий над пользователями
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IRepositoryWrapper _repositoryWrapper;

        private readonly IMapper _mapper;

        private const int _defaultActiveUsersStatisticsTop = 10;

        public UserService(
            IRepositoryWrapper repositoryWrapper,
            IMapper mapper)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
        }

        /// <summary>
        /// Возвращает набор пользователей
        /// </summary>
        /// <param name="filters">Набор пар ключ-значение</param>
        /// <returns>Набор пользователей</returns>
        /// <remarks>Фильтры не используются</remarks>
        public async Task<IEnumerable<UserReadModel>> List(IDictionary<string, string> filters)
        {
            var users = await Filter(_repositoryWrapper.User.FindAll(), filters).ToListAsync();
            var userDTOs = _mapper.Map<IEnumerable<UserDTO>>(users);
            var userReadModels = _mapper.Map<IEnumerable<UserReadModel>>(userDTOs);

            return userReadModels;
        }

        /// <summary>
        /// Возвращает пользователя по id платформы аутентификации
        /// </summary>
        /// <param name="identityUserId">id пользователя платформы аутентификации</param>
        /// <returns>Пользователь</returns>
        public async Task<UserWithEventsReadModel> RetrieveByIdentityUserId(Guid identityUserId)
        {
            var user = await GetUserWithIdentityId(identityUserId);

            if (user is null)
            {
                return null;
            }

            var userDTO = _mapper.Map<UserWithEventsDTO>(user);
            var userReadModel = _mapper.Map<UserWithEventsReadModel>(userDTO);
            userReadModel.OwnerId = (await GetUserOwnerAsync(userReadModel.Id, Owner.Type.File)).Id;

            return userReadModel;
        }

        /// <summary>
        /// Возвращает пользователя по id
        /// </summary>
        /// <param name="id">id пользователя</param>
        /// <returns>Пользователь</returns>
        public async Task<UserWithEventsReadModel> Retrieve(Guid id)
        {
            var user = await GetUserWithId(id);

            if (user is null)
            {
                return null;
            }

            var userDTO = _mapper.Map<UserWithEventsDTO>(user);
            var userReadModel = _mapper.Map<UserWithEventsReadModel>(userDTO);
            userReadModel.OwnerId = (await GetUserOwnerAsync(userReadModel.Id, Owner.Type.File)).Id;

            return userReadModel;
        }

        /// <summary>
        /// Создает пользователя
        /// </summary>
        /// <param name="createModel">Модель создания пользователя</param>
        /// <returns>Новый пользователь</returns>
        public async Task<UserWithEventsReadModel> Create(UserCreateModel createModel)
        {
            var user = _mapper.Map<ApplicationUser>(_mapper.Map<UserDTO>(createModel));

            _repositoryWrapper.User.Create(user);
            await _repositoryWrapper.SaveAsync();

            if (await CreateUserOwnerAsync(user.Id, Owner.Type.File) is null)
            {
                throw new Exception("Errors while creating owner");
            }

            var userReadModel = _mapper.Map<UserWithEventsReadModel>(_mapper.Map<UserWithEventsDTO>(user));
            userReadModel.OwnerId = (await GetUserOwnerAsync(userReadModel.Id, Owner.Type.File)).Id;

            return userReadModel;
        }

        /// <summary>
        /// Обновляет пользователя по email
        /// </summary>
        /// <param name="email">email пользователя</param>
        /// <param name="updateModel">Модель обновления пользователя</param>
        /// <returns>Обновленный пользователь</returns>
        public async Task<UserWithEventsReadModel> UpdateByEmail(string email, UserUpdateModel updateModel)
        {
            var user = await GetUserWithEmail(email);

            if (user is null)
            {
                throw new Exception($"User with email={email} is not found");
            }

            await CreateNewSubscriptions(updateModel.Events, user.Id);

            await CreateNewManagers(updateModel.ManagedEvents, user.Id);

            var userDTO = _mapper.Map<UserWithEventsDTO>(user);
            _mapper.Map(updateModel, userDTO);
            _mapper.Map(userDTO, user);

            _repositoryWrapper.User.Update(user);
            await _repositoryWrapper.SaveAsync();

            var userReadModel = _mapper.Map<UserWithEventsReadModel>(_mapper.Map<UserWithEventsDTO>(user));
            userReadModel.OwnerId = (await GetUserOwnerAsync(userReadModel.Id, Owner.Type.File)).Id;

            return userReadModel;
        }

        /// <summary>
        /// Обновляет пользователя
        /// </summary>
        /// <param name="identityUserId">id пользователя платформы аутентификации</param>
        /// <param name="updateModel">Модель обновления пользователя</param>
        /// <returns>Обновленный пользователь</returns>
        public async Task<UserWithEventsReadModel> UpdateByIdentityUserId(Guid identityUserId, UserUpdateModel updateModel)
        {
            var user = await GetUserWithIdentityId(identityUserId);

            if (user is null)
            {
                throw new Exception($"User with identityUserId={identityUserId} is not found");
            }

            await CreateNewSubscriptions(updateModel.Events, user.Id);

            await CreateNewManagers(updateModel.ManagedEvents, user.Id);

            var userDTO = _mapper.Map<UserWithEventsDTO>(user);
            _mapper.Map(updateModel, userDTO);
            _mapper.Map(userDTO, user);

            _repositoryWrapper.User.Update(user);
            await _repositoryWrapper.SaveAsync();

            var userReadModel = _mapper.Map<UserWithEventsReadModel>(_mapper.Map<UserWithEventsDTO>(user));
            userReadModel.OwnerId = (await GetUserOwnerAsync(userReadModel.Id, Owner.Type.File)).Id;

            return userReadModel;
        }

        /// <summary>
        /// Удаляет пользователя
        /// </summary>
        /// <param name="identityUserId">id пользователя платформы аутентификации</param>
        /// <returns></returns>
        public async Task DeleteByIdentityUserId(Guid identityUserId)
        {
            var user = await GetUserWithIdentityId(identityUserId);

            if (user is null)
            {
                throw new Exception($"User with identityUserId={identityUserId} is not found");
            }

            _repositoryWrapper.User.Delete(user);
            await _repositoryWrapper.SaveAsync();
        }

        /// <summary>
        /// Возвращает пользователем по его claims
        /// </summary>
        /// <param name="userClaims">Данные, определяющие пользователя</param>
        /// <returns>Пользователь</returns>
        public async Task<UserWithEventsReadModel> GetUserByClaims(ClaimsPrincipal userClaims)
        {
            var email = userClaims.Claims
                .Where(c => c.Type == ClaimTypes.Email)
                .SingleOrDefault();

            if (email is null || string.IsNullOrEmpty(email.Value))
            {
                throw new InvalidOperationException("User email is not specified");
            }

            var user = await GetUserByEmail(email.Value);

            if (user is null)
            {
                return null;
            }

            var userDTO = _mapper.Map<UserWithEventsDTO>(user);
            var userReadModel = _mapper.Map<UserWithEventsReadModel>(userDTO);
            userReadModel.OwnerId = (await GetUserOwnerAsync(userReadModel.Id, Owner.Type.File)).Id;

            return userReadModel;
        }

        /// <summary>
        /// Возвращает пользователя по email
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>Пользователь</returns>
        public async Task<UserWithEventsReadModel> GetUserByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new InvalidOperationException("User email is not specified");
            }

            var user = await GetUserWithEmail(email);

            if (user is null)
            {
                return null;
            }

            var userDTO = _mapper.Map<UserWithEventsDTO>(user);
            var userReadModel = _mapper.Map<UserWithEventsReadModel>(userDTO);
            userReadModel.OwnerId = (await GetUserOwnerAsync(userReadModel.Id, Owner.Type.File)).Id;

            return userReadModel;
        }

        /// <summary>
        /// Возвращает пользователя по id пользователя платформы аутентификации
        /// </summary>
        /// <param name="identityUserId">id пользователя платформы аутентификации</param>
        /// <returns>Пользователь</returns>
        public async Task<UserWithEventsReadModel> GetUserByIdentityUserId(Guid identityUserId)
        {
            if (Guid.Empty == identityUserId)
            {
                throw new InvalidOperationException("Identity id is not specified");
            }

            var user = await GetUserWithIdentityId(identityUserId);

            if (user is null)
            {
                return null;
            }

            var userDTO = _mapper.Map<UserWithEventsDTO>(user);
            var userReadModel = _mapper.Map<UserWithEventsReadModel>(userDTO);
            userReadModel.OwnerId = (await GetUserOwnerAsync(userReadModel.Id, Owner.Type.File)).Id;

            return userReadModel;
        }

        /// <summary>
        /// Возвращает статистику по пользователю
        /// </summary>
        /// <param name="identityUserId">id пользователя платформы аутентификации</param>
        /// <returns>Статистика по пользователю</returns>
        public async Task<IEnumerable<ChartData>> GetUserStatistics(Guid identityUserId)
        {
            var statistics = new List<ChartData>();
            var favoriteOrganizations = await GetFavoriteOrganizationsBySubscriptions(identityUserId);

            if (favoriteOrganizations is not null)
            {
                statistics.Add(favoriteOrganizations);
            }

            return statistics;
        }

        /// <summary>
        /// Возвращает статистику по пользователям
        /// </summary>
        /// <param name="filters">Набор пар для фильтрации пользователей</param>
        /// <returns>Статистика по пользователям</returns>
        public async Task<IEnumerable<ChartData>> GetUsersStatistics(IDictionary<string, string> filters)
        {
            var activeUsersTop = _defaultActiveUsersStatisticsTop;

            if (filters is not null)
            {
                if (filters.TryGetValue("activeUsersTop", out string activeUsersTopParam))
                {
                    if (int.TryParse(activeUsersTopParam, out int activeUsersTopValue))
                    {
                        activeUsersTop = activeUsersTopValue;
                    }
                }
            }

            var usersStatistics = new List<ChartData>();
            var mostActiveUsersStatistics = await GetMostActiveUsersStatistics(activeUsersTop);

            if (mostActiveUsersStatistics is not null)
            {
                usersStatistics.Add(mostActiveUsersStatistics);
            }

            return usersStatistics;
        }

        /// <summary>
        /// Возвращает пользователя с указанным id
        /// </summary>
        /// <param name="id">id пользователя</param>
        /// <returns>Пользователь с указанным id</returns>
        private async Task<ApplicationUser> GetUserEntityAsync(Guid id)
        {
            var user = await _repositoryWrapper.User
                .FindByCondition(user => user.Id == id)
                .SingleOrDefaultAsync();

            return user;
        }

        private async Task<ApplicationUser> GetUserWithEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new InvalidOperationException("User email is not specified");
            }

            var user = await _repositoryWrapper.User
                .FindByCondition(u => u.Email == email)
                .SingleOrDefaultAsync();

            return user;
        }

        private async Task<ApplicationUser> GetUserWithIdentityId(Guid identityUserId)
        {
            if (Guid.Empty == identityUserId)
            {
                throw new InvalidOperationException("Identity id is not specified");
            }

            var user = await _repositoryWrapper.User
                .FindByCondition(u => u.IdentityUserId == identityUserId)
                .SingleOrDefaultAsync();

            return user;
        }

        private async Task<ApplicationUser> GetUserWithId(Guid id)
        {
            if (Guid.Empty == id)
            {
                throw new InvalidOperationException("User id is not specified");
            }

            var user = await _repositoryWrapper.User
                .FindByCondition(u => u.Id == id)
                .SingleOrDefaultAsync();

            return user;
        }

        private async Task<ChartData> GetFavoriteOrganizationsBySubscriptions(Guid identityUserId)
        {
            var statistics = new ChartData
            {
                Title = "Доли организаций в подписках пользователя",
                Data = new List<ChartDataPiece>()
            };
            var user = await GetUserWithIdentityId(identityUserId);

            if (user is null)
            {
                return statistics;
            }

            var userSubscriptions = await _repositoryWrapper.Subscription
                .FindByCondition(s => s.ApplicationUserId == user.Id)
                .ToListAsync();

            var events = new List<Event>();

            foreach (var subscription in userSubscriptions)
            {
                events.Add(await _repositoryWrapper.Event
                    .FindByCondition(ev => ev.Id == subscription.EventId)
                    .SingleOrDefaultAsync());
            }

            var organizationsCountPerEvent = events
                .GroupBy(s => s.OrganizationId)
                .Select(g => new { orgId = g.Key, count = g.Count() })
                .ToDictionary(k => k.orgId is null ? -1 : k.orgId, i => i.count);

            foreach (var entry in organizationsCountPerEvent)
            {
                var name = "Без организации";

                if (entry.Key != -1)
                {
                    var organization = await _repositoryWrapper.Organization
                        .FindByCondition(org => org.Id == entry.Key.Value)
                        .SingleOrDefaultAsync();
                    name = organization.Name;
                }

                statistics.Data.Add(
                    new ChartDataPiece
                    {
                        X = name,
                        Y = entry.Value
                    });
            }

            return statistics;
        }

        private async Task<ChartData> GetMostActiveUsersStatistics(int number)
        {
            var statistics = new ChartData
            {
                Title = $"Топ самых активных пользователей по количеству посещаемых событий",
                Data = new List<ChartDataPiece>()
            };

            var eventsCountPerUser = await _repositoryWrapper.Subscription
                .FindAll()
                .GroupBy(s => s.ApplicationUserId)
                .Select(g => new { userId = g.Key, count = g.Count() })
                .OrderBy(g => g.count)
                .Take(number)
                .ToDictionaryAsync(k => k.userId, i => i.count);

            foreach (var entry in eventsCountPerUser)
            {
                var user = await GetUserEntityAsync(entry.Key);

                if (user is not null)
                {
                    statistics.Data.Add(
                    new ChartDataPiece
                    {
                        X = $"{user.Name} {user.Surname}",
                        Y = entry.Value
                    });
                }
            }

            return statistics;
        }

        private async Task CreateNewSubscriptions(IEnumerable<int> newIds, Guid userId)
        {
            await _repositoryWrapper.Subscription
                .FindByCondition(s => s.ApplicationUserId == userId)
                .ForEachAsync(s => _repositoryWrapper.Subscription.Delete(s));

            foreach (var eventId in newIds)
            {
                _repositoryWrapper.Subscription
                    .Create(new Subscription()
                    {
                        ApplicationUserId = userId,
                        EventId = eventId
                    });
            }
        }

        private async Task CreateNewManagers(IEnumerable<int> newIds, Guid userId)
        {
            await _repositoryWrapper.Management
                .FindByCondition(m => m.ApplicationUserId == userId)
                .ForEachAsync(m => _repositoryWrapper.Management.Delete(m));

            foreach (var eventId in newIds)
            {
                _repositoryWrapper.Management
                    .Create(new Management()
                    {
                        ApplicationUserId = userId,
                        EventId = eventId
                    });
            }
        }

        private static IQueryable<ApplicationUser> Filter(IQueryable<ApplicationUser> filesQuery, IDictionary<string, string> filters)
        {
            if (filters is not null)
            {
                // TODO: фильтрация
            }

            return filesQuery;
        }

        /// <summary>
        /// Создает владельца-пользователя
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <param name="type">Тип обладаемой сущности</param>
        /// <returns>Владелец</returns>
        private async Task<Owner> CreateUserOwnerAsync(Guid id, Owner.Type type)
        {
            var owner = new Owner
            {
                ApplicationUserId = id,
                OwnedType = type
            };

            _repositoryWrapper.Owner.Create(owner);
            await _repositoryWrapper.SaveAsync();

            return owner;
        }

        private async Task<Owner> GetUserOwnerAsync(Guid id, Owner.Type type)
        {
            var owner = await _repositoryWrapper.Owner
                    .FindByCondition(ow => ow.ApplicationUserId == id && ow.OwnedType == type)
                    .SingleOrDefaultAsync();

            if (owner is null)
            {
                owner = await CreateUserOwnerAsync(id, type);
            }

            return owner;
        }
    }
}
