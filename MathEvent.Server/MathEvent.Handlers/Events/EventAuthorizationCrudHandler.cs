using MathEvent.Contracts;
using MathEvent.Contracts.Services;
using MathEvent.Models.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace MathEvent.AuthorizationHandlers.Events
{
    /// <summary>
    /// Обработчик запроса на авторизацию для CRUD операций событий
    /// </summary>
    public class EventAuthorizationCrudHandler :
        AuthorizationHandler<OperationAuthorizationRequirement, EventWithUsersReadModel>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;

        private readonly IUserService _userService;

        public EventAuthorizationCrudHandler(
            IRepositoryWrapper repositoryWrapper,
            IUserService userService)
        {
            _repositoryWrapper = repositoryWrapper;
            _userService = userService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            EventWithUsersReadModel resource)
        {
            var user = await _userService.GetUserAsync(context.User);
            var userManagedEventIds = await _repositoryWrapper
                .Management
                .FindByCondition(m => m.ApplicationUserId == user.Id)
                .Select(m => m.EventId)
                .ToListAsync();

            if (requirement.Name == Operations.Update.Name
                || requirement.Name == Operations.Delete.Name)
            {
                if (userManagedEventIds.Contains(resource.Id))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }

            context.Succeed(requirement);
        }
    }
}
