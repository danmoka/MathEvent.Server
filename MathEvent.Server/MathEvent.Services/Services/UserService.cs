using AutoMapper;
using MathEvent.Contracts;
using MathEvent.Contracts.Services;
using MathEvent.DTOs.Users;
using MathEvent.Entities.Entities;
using MathEvent.Models.Email;
using MathEvent.Models.Others;
using MathEvent.Models.Users;
using Microsoft.AspNetCore.Identity;
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

        private readonly IEmailService _emailSender;

        private readonly UserManager<ApplicationUser> _userManager;

        private const int _defaultActiveUsersStatisticsTop = 10;

        public UserService(
            IRepositoryWrapper repositoryWrapper,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            IEmailService emailSender)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        /// <summary>
        /// Возвращает набор пользователей
        /// </summary>
        /// <param name="filters">Набор пар ключ-значение</param>
        /// <returns>Набор пользователей</returns>
        /// <remarks>Фильтры не используются</remarks>
        public async Task<IEnumerable<UserReadModel>> ListAsync(IDictionary<string, string> filters)
        {
            var users = await Filter(_repositoryWrapper.User.FindAll(), filters).ToListAsync();
            var userDTOs = _mapper.Map<IEnumerable<UserDTO>>(users);
            var userReadModels = _mapper.Map<IEnumerable<UserReadModel>>(userDTOs);

            return userReadModels;
        }

        /// <summary>
        /// Возвращает пользователя с указанным id
        /// </summary>
        /// <param name="id">id пользователя</param>
        /// <returns>Пользователь</returns>
        public async Task<UserWithEventsReadModel> RetrieveAsync(string id)
        {
            var user = await GetUserEntityAsync(id);

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
        public async Task<UserWithEventsReadModel> CreateAsync(UserCreateModel createModel)
        {
            var user = _mapper.Map<ApplicationUser>(_mapper.Map<UserDTO>(createModel));
            var createResult = await _repositoryWrapper.User.CreateAsync(user, createModel.Password);

            if (!createResult.Succeeded)
            {
                throw new Exception(createResult.Errors.ToString());
            }

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
        /// Обновляет пользователя
        /// </summary>
        /// <param name="id">id пользователя</param>
        /// <param name="updateModel">Модель обновления пользователя</param>
        /// <returns>Обновленный пользователь</returns>
        public async Task<UserWithEventsReadModel> UpdateAsync(string id, UserUpdateModel updateModel)
        {
            var user = await GetUserEntityAsync(id);

            if (user is null)
            {
                throw new Exception($"User with id={id} is not found");
            }

            await CreateNewSubscriptions(updateModel.Events, id);

            await CreateNewManagers(updateModel.ManagedEvents, id);

            var userDTO = _mapper.Map<UserWithEventsDTO>(user);
            _mapper.Map(updateModel, userDTO);
            _mapper.Map(userDTO, user);

            var updateResult = await _repositoryWrapper.User.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                throw new Exception(updateResult.Errors.ToString());
            }

            await _repositoryWrapper.SaveAsync();

            var userReadModel = _mapper.Map<UserWithEventsReadModel>(_mapper.Map<UserWithEventsDTO>(user));
            userReadModel.OwnerId = (await GetUserOwnerAsync(userReadModel.Id, Owner.Type.File)).Id;

            return userReadModel;
        }

        /// <summary>
        /// Удаляет пользователя
        /// </summary>
        /// <param name="id">id пользователя</param>
        /// <returns></returns>
        public async Task DeleteAsync(string id)
        {
            var user = await GetUserEntityAsync(id);

            if (user is null)
            {
                throw new Exception($"User with id={id} is not found");
            }

            var deleteResult = await _repositoryWrapper.User.DeleteAsync(user);

            if (deleteResult.Succeeded)
            {
                await _repositoryWrapper.SaveAsync();
            }
            else
            {
                throw new Exception(deleteResult.Errors.ToString());
            }
        }

        /// <summary>
        /// Возвращает пользователем по его claims
        /// </summary>
        /// <param name="userPrincipal">Данные, определяющие пользователя</param>
        /// <returns>Пользователь</returns>
        public async Task<ApplicationUser> GetUserAsync(ClaimsPrincipal userPrincipal)
        {
            var user = await _userManager.GetUserAsync(userPrincipal);

            return user;
        }

        /// <summary>
        /// Возвращает пользователя по email
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>Пользователь</returns>
        public async Task<ApplicationUser> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            return user;
        }

        /// <summary>
        /// Отправляет токен смены пароля на электронную почту пользователя
        /// </summary>
        /// <param name="email">Адрес электронной почты пользователя</param>
        /// <returns></returns>
        public async Task ForgotPasswordAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new Exception("The user's email address is empty");
            }

            var user = await GetUserByEmail(email);

            if (user is null)
            {
                return;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var message = new Message(
                new string[] { user.Email },
                "Смена пароля",
                $"Подтверждающий код: {token}");

            _emailSender.SendEmail(message);
        }

        /// <summary>
        /// Меняет пароль пользователя
        /// </summary>
        /// <param name="resetModel">Модель смены пароля</param>
        /// <returns></returns>
        public async Task ResetPasswordAsync(ForgotPasswordResetModel resetModel)
        {
            var user = await GetUserByEmail(resetModel.Email);

            if (user is null)
            {
                return;
            }

            var result = await _userManager.ResetPasswordAsync(user, resetModel.Token, resetModel.Password);

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.ToString());
            }

            return;
        }

        /// <summary>
        /// Возвращает статистику по пользователю
        /// </summary>
        /// <param name="id">id пользователя</param>
        /// <returns>Статистика по пользователю</returns>
        public async Task<IEnumerable<ChartData>> GetUserStatistics(string id)
        {
            var statistics = new List<ChartData>();
            var favoriteOrganizations = await GetFavoriteOrganizationsBySubscriptions(id);

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
        private async Task<ApplicationUser> GetUserEntityAsync(string id)
        {
            var user = await _repositoryWrapper.User
                .FindByCondition(user => user.Id == id)
                .SingleOrDefaultAsync();

            return user;
        }

        private async Task<ChartData> GetFavoriteOrganizationsBySubscriptions(string userId)
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

            var statistics = new ChartData
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
                        X = $"{user.Name[0]}. {user.Surname} ({user.UserName})",
                        Y = entry.Value
                    });
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
            // TODO: вынести работу по созданию owner в profile. При попытке получения owner'а создавать его, если его нет
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
