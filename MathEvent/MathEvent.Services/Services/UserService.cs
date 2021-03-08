using AutoMapper;
using MathEvent.Contracts;
using MathEvent.Converters.Identities.DTOs;
using MathEvent.Converters.Identities.Models;
using MathEvent.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Service.Messages;
using Service.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Services
{
    /// <summary>
    /// Сервис по выполнению действий над пользователями
    /// </summary>
    public class UserService : IUserService
    {
        private IRepositoryWrapper _repositoryWrapper { get; set; }

        private IMapper _mapper { get; set; }

        public UserService(IRepositoryWrapper repositoryWrapper, IMapper mapper)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserReadModel>> ListAsync()
        {
            var users = await _repositoryWrapper.User
                .FindAll()
                .ToListAsync();

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

                return _mapper.Map<UserWithEventsReadModel>(userDTO);
            }

            return null;
        }

        public async Task<AResult<IMessage, ApplicationUser>> CreateAsync(UserCreateModel createModel)
        {
            var user = _mapper.Map<ApplicationUser>(_mapper.Map<UserDTO>(createModel));

            if (user is not null)
            {
                var createResult = await _repositoryWrapper.User
                    .CreateAsync(user, createModel.Password);
                // TODO: AddToRoleAsync

                if (createResult.Succeeded)
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
                        Messages = UserMessageResult.GetMessagesFromErrors(createResult.Errors),
                        Entity = user
                    };
                }
            }

            return new UserMessageResult
            {
                Succeeded = false,
                Messages = new List<SimpleMessage>
                {
                    new SimpleMessage
                    {
                        Message = "Error when mapping model"
                    }
                }
            };
        }

        public async Task<AResult<IMessage, ApplicationUser>> UpdateAsync(string id, UserUpdateModel updateModel)
        {
            var user = await GetUserEntityAsync(id);
            // TODO: ? AddToRoleAsync

            if (user is not null)
            {
                await CreateNewSubscriptions(updateModel.Events, id);
                var userDTO = _mapper.Map<UserWithEventsDTO>(user);
                _mapper.Map(updateModel, userDTO);
                _mapper.Map(userDTO, user);
                var updateResult = await _repositoryWrapper.User
                    .UpdateAsync(user);

                if (updateResult.Succeeded)
                {
                    await _repositoryWrapper.SaveAsync();

                    return new UserMessageResult { Succeeded = true };
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

        public async Task<AResult<IMessage, ApplicationUser>> DeleteAsync(string id)
        {
            var user = await GetUserEntityAsync(id);

            if (user is not null)
            {
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

        public async Task<ApplicationUser> GetUserEntityAsync(string id)
        {
            return await _repositoryWrapper.User
                .FindByCondition(user => user.Id == id)
                .SingleOrDefaultAsync();
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
    }
}
