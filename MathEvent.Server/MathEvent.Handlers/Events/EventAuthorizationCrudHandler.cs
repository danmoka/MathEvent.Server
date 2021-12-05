using MathEvent.Contracts.Services;
using MathEvent.Models.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
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
        private readonly IUserService _userService;

        public EventAuthorizationCrudHandler(
            IUserService userService)
        {
            _userService = userService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            EventWithUsersReadModel resource)
        {
            var user = await _userService.GetUserByClaims(context.User);

            if (user is null)
            {
                context.Fail();
            }

            if (requirement.Name == Operations.Update.Name
                || requirement.Name == Operations.Delete.Name)
            {
                if (user.ManagedEvents.Where(ev => ev.Id == resource.Id).Any())
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
