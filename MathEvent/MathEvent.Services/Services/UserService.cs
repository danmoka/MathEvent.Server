using AutoMapper;
using MathEvent.Contracts;
using MathEvent.Converters.Identities.DTOs;
using MathEvent.Converters.Identities.Models;
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

        private readonly IOwnerService _ownerService;

        public UserService(
            IRepositoryWrapper repositoryWrapper,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            IOwnerService ownerService)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _userManager = userManager;
            _ownerService = ownerService;
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
                userReadModel.OwnerId = (await _ownerService.GetUserOwnerAsync(userReadModel.Id, Owner.Type.File)).Id;

                return userReadModel;
            }

            return null;
        }

        public async Task<AResult<IMessage, ApplicationUser>> CreateAsync(UserCreateModel createModel)
        {
            var user = _mapper.Map<ApplicationUser>(_mapper.Map<UserDTO>(createModel));

            if (user is null)
            {
                return new UserMessageResult
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
                return new UserMessageResult
                {
                    Succeeded = false,
                    Messages = UserMessageResult.GetMessagesFromErrors(createResult.Errors),
                    Entity = user
                };
            }

            await _repositoryWrapper.SaveAsync();

            if (await _ownerService.CreateUserOwner(user.Id, Owner.Type.File) is null)
            {
                return new UserMessageResult
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

            return new UserMessageResult
            {
                Succeeded = true,
                Entity = user
            };
        }

        public async Task<AResult<IMessage, ApplicationUser>> UpdateAsync(string id, UserUpdateModel updateModel)
        {
            var user = await GetUserEntityAsync(id);
            // TODO: ? AddToRoleAsync

            if (user is not null)
            {
                return new UserMessageResult
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
            var userDTO = _mapper.Map<UserWithEventsDTO>(user);
            _mapper.Map(updateModel, userDTO);
            _mapper.Map(userDTO, user);
            var updateResult = await _repositoryWrapper.User
                .UpdateAsync(user);

            if (updateResult.Succeeded)
            {
                await _repositoryWrapper.SaveAsync();

                return new UserMessageResult 
                { 
                    Succeeded = true,
                    Entity = user
                };
            }
            else
            {
                return new UserMessageResult
                {
                    Succeeded = false,
                    Messages = UserMessageResult.GetMessagesFromErrors(updateResult.Errors)
                };
            }
        }

        public async Task<AResult<IMessage, ApplicationUser>> DeleteAsync(string id)
        {
            var user = await GetUserEntityAsync(id);

            if (user is not null)
            {
                return new UserMessageResult
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

                return new UserMessageResult { Succeeded = true };
            }
            else
            {
                return new UserMessageResult
                {
                    Succeeded = false,
                    Messages = UserMessageResult.GetMessagesFromErrors(deleteResult.Errors)
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

        private static IQueryable<ApplicationUser> Filter(IQueryable<ApplicationUser> filesQuery, IDictionary<string, string> filters)
        {
            if (filters is not null)
            {
                // TODO: фильтрация
            }

            return filesQuery;
        }
    }
}
