using MathEvent.Constants;
using MathEvent.Contracts.Services;
using MathEvent.Models.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
            if (requirement.Name == Operations.Update.Name
                || requirement.Name == Operations.Delete.Name)
            {
                var user = await _userService.GetUserByClaims(context.User);

                if (user is not null)
                {
                    if (user.ManagedEvents.Where(ev => ev.Id == resource.Id).Any())
                    {
                        context.Succeed(requirement);
                        return;
                    }
                }

                var roleClaim = context.User.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .SingleOrDefault();

                if (roleClaim is not null)
                {
                    var roles = JsonConvert.DeserializeObject<IList<string>>(roleClaim.Value);

                    if (roles.Contains(MathEventRoles.Administrator))
                    {
                        context.Succeed(requirement);
                        return;
                    }
                }
            }

            context.Fail();
        }
    }
}
