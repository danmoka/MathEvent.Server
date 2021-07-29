using AutoMapper;
using MathEvent.Contracts;
using MathEvent.Converters.Identities.DTOs;
using MathEvent.Converters.Identities.Models;
using MathEvent.Converters.Others;
using MathEvent.Entities.Entities;
using MathEvent.Services.Messages;
using MathEvent.Services.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        private readonly UserManager<ApplicationUser> _userManager;

        // Циклическая зависимость. Мб стоит также убрать во стальных сервисах, например, то, что используется для статистики
        //private readonly IEventService _eventService;

        //private readonly IOrganizationService _organizationService;

        public UserService(
            IRepositoryWrapper repositoryWrapper,
            IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<IEnumerable<UserReadModel>> ListAsync(IDictionary<string, string> filters)
        {
            var users = await Filter(_repositoryWrapper.User.FindAll(), filters).ToListAsync();

            if (users is not null)
            {
                var usersDTO = _mapper.Map<IEnumerable<UserDTO>>(users);

                return _mapper.Map<IEnumerable<UserReadModel>>(usersDTO);
            }

            return null;
        }

        public async Task<UserWithEventsReadModel> RetrieveAsync(string id)
        {
            var user = await GetUserEntityAsync(id);

            if (user is not null)
            {
                var userDTO = _mapper.Map<UserWithEventsDTO>(user);
                var userReadModel = _mapper.Map<UserWithEventsReadModel>(userDTO);
                userReadModel.OwnerId = (await GetUserOwnerAsync(userReadModel.Id, Owner.Type.File)).Id;

                return userReadModel;
            }

            return null;
        }

        public async Task<AResult<IMessage, UserWithEventsReadModel>> CreateAsync(UserCreateModel createModel)
        {
            var user = _mapper.Map<ApplicationUser>(_mapper.Map<UserDTO>(createModel));

            if (user is null)
            {
                return new MessageResult<UserWithEventsReadModel>
                {
                    Succeeded = false,
                    Messages = new List<SimpleMessage>()
                    {
                        new SimpleMessage
                        {
                            Message = $"Errors when mapping model {createModel.Name}"
                        }
                    }
                };
            }

            var createResult = await _repositoryWrapper.User
                    .CreateAsync(user, createModel.Password);

            if (!createResult.Succeeded)
            {
                return new MessageResult<UserWithEventsReadModel>
                {
                    Succeeded = false,
                    Messages = ResultUtils.MapIdentityErrorsToMessages(createResult.Errors)
                };
            }

            await _repositoryWrapper.SaveAsync();

            if (await CreateUserOwnerAsync(user.Id, Owner.Type.File) is null)
            {
                return new MessageResult<UserWithEventsReadModel>
                {
                    Succeeded = false,
                    Messages = new List<SimpleMessage>()
                    {
                        new SimpleMessage
                        {
                            Message = $"Errors when creating an owner for user with id = {user.Id}"
                        }
                    }
                };
            }

            UserWithEventsReadModel userReadModel = _mapper.Map<UserWithEventsReadModel>(_mapper.Map<UserWithEventsDTO>(user));
            userReadModel.OwnerId = (await GetUserOwnerAsync(userReadModel.Id, Owner.Type.File)).Id;

            return new MessageResult<UserWithEventsReadModel>
            {
                Succeeded = true,
                Entity = userReadModel
            };
        }

        public async Task<AResult<IMessage, UserWithEventsReadModel>> UpdateAsync(string id, UserUpdateModel updateModel)
        {
            var user = await GetUserEntityAsync(id);
            // TODO: ? AddToRoleAsync

            if (user is null)
            {
                return new MessageResult<UserWithEventsReadModel>
                {
                    Succeeded = false,
                    Messages = new List<SimpleMessage>
                    {
                        new SimpleMessage
                        {
                            Code = "404",
                            Message = $"User with the ID {id} not found"
                        }
                    }
                };
            }

            await CreateNewSubscriptions(updateModel.Events, id);
            await CreateNewManagers(updateModel.ManagedEvents, id);
            var userDTO = _mapper.Map<UserWithEventsDTO>(user);
            _mapper.Map(updateModel, userDTO);
            _mapper.Map(userDTO, user);
            var updateResult = await _repositoryWrapper.User
                .UpdateAsync(user);
            await _repositoryWrapper.SaveAsync();

            UserWithEventsReadModel userReadModel = _mapper.Map<UserWithEventsReadModel>(_mapper.Map<UserWithEventsDTO>(user));
            userReadModel.OwnerId = (await GetUserOwnerAsync(userReadModel.Id, Owner.Type.File)).Id;

            if (updateResult.Succeeded)
            {
                await _repositoryWrapper.SaveAsync();

                return new MessageResult<UserWithEventsReadModel>
                {
                    Succeeded = true,
                    Entity = userReadModel
                };
            }
            else
            {
                return new MessageResult<UserWithEventsReadModel>
                {
                    Succeeded = false,
                    Messages = ResultUtils.MapIdentityErrorsToMessages(updateResult.Errors)
                };
            }
        }

        public async Task<AResult<IMessage, UserWithEventsReadModel>> DeleteAsync(string id)
        {
            var user = await GetUserEntityAsync(id);

            if (user is null)
            {
                return new MessageResult<UserWithEventsReadModel>
                {
                    Succeeded = false,
                    Messages = new List<SimpleMessage>
                    {
                        new SimpleMessage
                        {
                            Code = "404",
                            Message = $"User with the ID {id} not found"
                        }
                    }
                };
            }

            var deleteResult = await _repositoryWrapper.User
                    .DeleteAsync(user);

            if (deleteResult.Succeeded)
            {
                await _repositoryWrapper.SaveAsync();

                return new MessageResult<UserWithEventsReadModel> { Succeeded = true };
            }
            else
            {
                return new MessageResult<UserWithEventsReadModel>
                {
                    Succeeded = false,
                    Messages = ResultUtils.MapIdentityErrorsToMessages(deleteResult.Errors)
                };
            }
        }

        public async Task<ApplicationUser> GetUserEntityAsync(string id)
        {
            return await _repositoryWrapper.User
                .FindByCondition(user => user.Id == id)
                .SingleOrDefaultAsync();
        }

        public async Task<ApplicationUser> GetCurrentUserAsync(ClaimsPrincipal user)
        {
            return await _userManager.GetUserAsync(user);
        }

        public async Task<IEnumerable<SimpleStatistics>> GetUserStatistics(string id)
        {
            ICollection<SimpleStatistics> statistics = new List<SimpleStatistics>
            {
                await GetFavoriteOrganizationsBySubscriptions(id)
            };

            return statistics;
        }

        public async Task<SimpleStatistics> GetFavoriteOrganizationsBySubscriptions(string userId)
        {
            var userSubscriptions = await _repositoryWrapper.Subscription
                .FindByCondition(s => s.ApplicationUserId == userId)
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

            var statistics = new SimpleStatistics
            {
                Title = "Доли организаций в подписках пользователя",
                Data = new List<ChartDataPiece>()
            };

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

        public async Task<IEnumerable<SimpleStatistics>> GetSimpleStatistics(IDictionary<string, string> filters)
        {
            int activeUsersTop = 10;

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

            ICollection<SimpleStatistics> simpleStatistics = new List<SimpleStatistics>
            {
                await GetMostActiveUsersStatistics(activeUsersTop),
            };

            return simpleStatistics;
        }

        private async Task<SimpleStatistics> GetMostActiveUsersStatistics(int number)
        {
            var statistics = new SimpleStatistics
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
                var userEntity = await GetUserEntityAsync(entry.Key);

                statistics.Data.Add(
                    new ChartDataPiece
                    {
                        X = $"{userEntity.Name[0]}. {userEntity.Surname} ({userEntity.UserName})",
                        Y = entry.Value
                    });
            }

            return statistics;
        }

        private async Task CreateNewSubscriptions(IEnumerable<int> newIds, string userId)
        {
            await _repositoryWrapper.Subscription
                .FindByCondition(s => s.ApplicationUserId == userId)
                .ForEachAsync(s => _repositoryWrapper.Subscription.Delete(s));

            foreach (var eventId in newIds)
            {
                await _repositoryWrapper.Subscription
                    .CreateAsync(new Subscription()
                    {
                        ApplicationUserId = userId,
                        EventId = eventId
                    });
            }
        }

        private async Task CreateNewManagers(IEnumerable<int> newIds, string userId)
        {
            await _repositoryWrapper.Management
                .FindByCondition(m => m.ApplicationUserId == userId)
                .ForEachAsync(m => _repositoryWrapper.Management.Delete(m));

            foreach (var eventId in newIds)
            {
                await _repositoryWrapper.Management
                    .CreateAsync(new Management()
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
        private async Task<Owner> CreateUserOwnerAsync(string id, Owner.Type type)
        {
            var owner = await _repositoryWrapper.Owner.CreateAsync(
                new Owner
                {
                    ApplicationUserId = id,
                    OwnedType = type
                });
            await _repositoryWrapper.SaveAsync();

            return owner;
        }

        private async Task<Owner> GetUserOwnerAsync(string id, Owner.Type type)
        {
            var owner = _repositoryWrapper.Owner
                    .FindByCondition(ow => ow.ApplicationUserId == id && ow.OwnedType == type)
                    .SingleOrDefault();

            if (owner is null)
            {
                owner = await CreateUserOwnerAsync(id, type);
            }

            return owner;
        }
    }
}
