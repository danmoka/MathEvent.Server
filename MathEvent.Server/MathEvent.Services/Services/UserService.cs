using AutoMapper;
using MathEvent.Contracts;
using MathEvent.Converters.Identities.DTOs;
using MathEvent.Converters.Identities.Models;
using MathEvent.Converters.Others;
using MathEvent.Entities.Entities;
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
    public class UserService
    {
        private readonly IRepositoryWrapper _repositoryWrapper;

        private readonly IMapper _mapper;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IEmailSender _emailSender;

        private const int _defaultActiveUsersStatisticsTop = 10;

        public UserService(
            IRepositoryWrapper repositoryWrapper,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        /// <summary>
        /// Возвращает результат с набором пользователей
        /// </summary>
        /// <param name="filters">Набор пар ключ-значение</param>
        /// <returns>Результат с набором пользователей</returns>
        /// <remarks>Фильтры не используются</remarks>
        public async Task<IResult<IMessage, IEnumerable<UserReadModel>>> ListAsync(IDictionary<string, string> filters)
        {
            var users = await Filter(_repositoryWrapper.User.FindAll(), filters).ToListAsync();

            if (users is not null)
            {
                var usersDTO = _mapper.Map<IEnumerable<UserDTO>>(users);

                return ResultFactory.GetSuccessfulResult(_mapper.Map<IEnumerable<UserReadModel>>(usersDTO));
            }

            return ResultFactory.GetUnsuccessfulMessageResult<IEnumerable<UserReadModel>>(new List<IMessage>()
            {
                MessageFactory.GetSimpleMessage("402", "The list of users is empty")
            });
        }

        /// <summary>
        /// Возвращает результат с пользователем с указанным id
        /// </summary>
        /// <param name="id">id пользователя</param>
        /// <returns>Результат с пользователем</returns>
        public async Task<IResult<IMessage, UserWithEventsReadModel>> RetrieveAsync(string id)
        {
            var userResult = await GetUserEntityAsync(id);

            if (!userResult.Succeeded)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<UserWithEventsReadModel>(userResult.Messages);
            }

            var user = userResult.Entity;

            if (user is null)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<UserWithEventsReadModel>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage("404", $"User with id = {id} not found")
                });
            }

            var userDTO = _mapper.Map<UserWithEventsDTO>(user);
            var userReadModel = _mapper.Map<UserWithEventsReadModel>(userDTO);
            userReadModel.OwnerId = (await GetUserOwnerAsync(userReadModel.Id, Owner.Type.File)).Id;

            return ResultFactory.GetSuccessfulResult(userReadModel);
        }

        /// <summary>
        /// Создает пользователя
        /// </summary>
        /// <param name="createModel">Модель создания пользователя</param>
        /// <returns>Результат создания пользователя</returns>
        public async Task<IResult<IMessage, UserWithEventsReadModel>> CreateAsync(UserCreateModel createModel)
        {
            var user = _mapper.Map<ApplicationUser>(_mapper.Map<UserDTO>(createModel));

            if (user is null)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<UserWithEventsReadModel>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage(null, $"Errors when mapping model {createModel.Name}")
                });
            }

            var createResult = await _repositoryWrapper.User
                    .CreateAsync(user, createModel.Password);

            if (!createResult.Succeeded)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<UserWithEventsReadModel>(ResultUtils.MapIdentityErrorsToMessages(createResult.Errors));
            }

            await _repositoryWrapper.SaveAsync();

            if (await CreateUserOwnerAsync(user.Id, Owner.Type.File) is null)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<UserWithEventsReadModel>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage(null, $"Errors when creating an owner for user with id = {user.Id}")
                });
            }

            UserWithEventsReadModel userReadModel = _mapper.Map<UserWithEventsReadModel>(_mapper.Map<UserWithEventsDTO>(user));
            userReadModel.OwnerId = (await GetUserOwnerAsync(userReadModel.Id, Owner.Type.File)).Id;

            return ResultFactory.GetSuccessfulResult(userReadModel);
        }

        /// <summary>
        /// Обновляет пользователя
        /// </summary>
        /// <param name="id">id пользователя</param>
        /// <param name="updateModel">Модель обновления пользователя</param>
        /// <returns>Результат обновления пользователя</returns>
        public async Task<IResult<IMessage, UserWithEventsReadModel>> UpdateAsync(string id, UserUpdateModel updateModel)
        {
            var userResult = await GetUserEntityAsync(id);

            if (!userResult.Succeeded)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<UserWithEventsReadModel>(userResult.Messages);
            }

            var user = userResult.Entity;
            // TODO: ? AddToRoleAsync

            if (user is null)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<UserWithEventsReadModel>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage("404", $"User with the ID {id} not found")
                });
            }

            await CreateNewSubscriptions(updateModel.Events, id);
            await CreateNewManagers(updateModel.ManagedEvents, id);
            var userDTO = _mapper.Map<UserWithEventsDTO>(user);
            _mapper.Map(updateModel, userDTO);
            _mapper.Map(userDTO, user);

            var updateResult = await _repositoryWrapper.User.UpdateAsync(user);
            await _repositoryWrapper.SaveAsync();

            UserWithEventsReadModel userReadModel = _mapper.Map<UserWithEventsReadModel>(_mapper.Map<UserWithEventsDTO>(user));
            userReadModel.OwnerId = (await GetUserOwnerAsync(userReadModel.Id, Owner.Type.File)).Id;

            if (updateResult.Succeeded)
            {
                await _repositoryWrapper.SaveAsync();

                return ResultFactory.GetSuccessfulResult(userReadModel);
            }
            else
            {
                return ResultFactory.GetUnsuccessfulMessageResult<UserWithEventsReadModel>(ResultUtils.MapIdentityErrorsToMessages(updateResult.Errors));
            }
        }

        /// <summary>
        /// Удаляет пользователя
        /// </summary>
        /// <param name="id">id пользователя</param>
        /// <returns>Результат удаления пользователя</returns>
        public async Task<IResult<IMessage, UserWithEventsReadModel>> DeleteAsync(string id)
        {
            var userResult = await GetUserEntityAsync(id);

            if (!userResult.Succeeded)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<UserWithEventsReadModel>(userResult.Messages);
            }

            var user = userResult.Entity;

            if (user is null)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<UserWithEventsReadModel>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage("404", $"User with the ID {id} not found")
                });
            }

            var deleteResult = await _repositoryWrapper.User.DeleteAsync(user);

            if (deleteResult.Succeeded)
            {
                await _repositoryWrapper.SaveAsync();

                return ResultFactory.GetSuccessfulResult((UserWithEventsReadModel)null);
            }
            else
            {
                return ResultFactory.GetUnsuccessfulMessageResult<UserWithEventsReadModel>(ResultUtils.MapIdentityErrorsToMessages(deleteResult.Errors));
            }
        }

        /// <summary>
        /// Возвращает результат с пользователем с указанным id
        /// </summary>
        /// <param name="id">id пользователя</param>
        /// <returns>Результат с пользователем с указанным id</returns>
        public async Task<IResult<IMessage, ApplicationUser>> GetUserEntityAsync(string id)
        {
            var user = await _repositoryWrapper.User
                .FindByCondition(user => user.Id == id)
                .SingleOrDefaultAsync();

            if (user is not null)
            {
                return ResultFactory.GetSuccessfulResult(user);
            }

            return ResultFactory.GetUnsuccessfulMessageResult<ApplicationUser>(new List<IMessage>()
            {
                MessageFactory.GetSimpleMessage("404", $"User with id = {id} is not found")
            });
        }

        /// <summary>
        /// Возвращает результат с текущим пользователем
        /// </summary>
        /// <param name="userPrincipal">Данные, определяющие текущего пользователя</param>
        /// <returns>Результат с текущим пользователем</returns>
        public async Task<IResult<IMessage, ApplicationUser>> GetCurrentUserAsync(ClaimsPrincipal userPrincipal)
        {
            var user = await _userManager.GetUserAsync(userPrincipal);

            if (user is not null)
            {
                return ResultFactory.GetSuccessfulResult(user);
            }

            return ResultFactory.GetUnsuccessfulMessageResult<ApplicationUser>(new List<IMessage>()
            {
                MessageFactory.GetSimpleMessage("404", $"User is not found")
            });
        }

        /// <summary>
        /// Отправляет токен смены пароля на электронную почту пользователя
        /// </summary>
        /// <param name="email">Адрес электронной почты пользователя</param>
        /// <returns>Результат генерации и отправки токена</returns>
        public async Task<IResult<IMessage, string>> ForgotPasswordAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return ResultFactory.GetUnsuccessfulMessageResult<string>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage("400", "The user's email address is empty")
                });
            }

            var user = await _repositoryWrapper
                .User
                .FindByCondition(u => u.Email == email)
                .SingleOrDefaultAsync();

            if (user is null)
            {
                return ResultFactory.GetSuccessfulResult("Ok");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var message = new Message(
                new string[] { user.Email },
                "Смена пароля",
                $"Подтверждающий код: {token}");
            _emailSender.SendEmail(message);

            return ResultFactory.GetSuccessfulResult("Ok");
        }

        /// <summary>
        /// Меняет пароль пользователя
        /// </summary>
        /// <param name="resetModel">Модель смены пароля</param>
        /// <returns>Результат смены пароля</returns>
        public async Task<IResult<IMessage, string>> ResetPasswordAsync(ForgotPasswordResetModel resetModel)
        {
            var user = await _userManager.FindByEmailAsync(resetModel.Email);

            if (user is null)
            {
                return ResultFactory.GetSuccessfulResult("Ok");
            }

            var result = await _userManager.ResetPasswordAsync(user, resetModel.Token, resetModel.Password);

            if (result.Succeeded)
            {
                return ResultFactory.GetSuccessfulResult("Password changed");
            }
            else
            {
                return ResultFactory.GetUnsuccessfulMessageResult<string>(
                    new List<IMessage>(result.Errors
                    .Select(er => MessageFactory.GetIdentityMessage(er))));
            }
        }

        /// <summary>
        /// Возвращает результат со статистикой по пользователю
        /// </summary>
        /// <param name="id">id пользователя</param>
        /// <returns>Результат со статистикой по пользователю</returns>
        public async Task<IResult<IMessage, IEnumerable<SimpleStatistics>>> GetUserStatistics(string id)
        {
            var statistics = new List<SimpleStatistics>();
            var favoriteOrganizations = await GetFavoriteOrganizationsBySubscriptions(id);

            if (favoriteOrganizations is not null)
            {
                statistics.Add(favoriteOrganizations);
            }

            if (statistics.Count < 1)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<IEnumerable<SimpleStatistics>>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage("400", $"Errors when getting a statistics for user with id = {id}")
                });
            }

            return ResultFactory.GetSuccessfulResult(statistics.AsEnumerable());
        }

        public async Task<IResult<IMessage, IEnumerable<SimpleStatistics>>> GetSimpleStatistics(IDictionary<string, string> filters)
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

            var simpleStatistics = new List<SimpleStatistics>();
            var mostActiveUsersStatistics = await GetMostActiveUsersStatistics(activeUsersTop);

            if (mostActiveUsersStatistics is not null)
            {
                simpleStatistics.Add(mostActiveUsersStatistics);
            }

            if (simpleStatistics.Count < 1)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<IEnumerable<SimpleStatistics>>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage("400", "Errors when getting a statistics for users")
                });
            }

            return ResultFactory.GetSuccessfulResult(simpleStatistics.AsEnumerable());
        }

        private async Task<SimpleStatistics> GetFavoriteOrganizationsBySubscriptions(string userId)
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
                var userResult = await GetUserEntityAsync(entry.Key);

                if (userResult.Succeeded)
                {
                    var userEntity = userResult.Entity;

                    if (userEntity is not null)
                    {
                        statistics.Data.Add(
                        new ChartDataPiece
                        {
                            X = $"{userEntity.Name[0]}. {userEntity.Surname} ({userEntity.UserName})",
                            Y = entry.Value
                        });
                    }
                }
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
