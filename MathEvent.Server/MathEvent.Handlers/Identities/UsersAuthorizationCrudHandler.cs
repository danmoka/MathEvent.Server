using MathEvent.Constants;
using MathEvent.Contracts.Services;
using MathEvent.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MathEvent.AuthorizationHandlers.Identities
{
    /// <summary>
    /// Обработчик запроса на авторизацию для CRUD операций пользователей
    /// </summary>
    public class UsersAuthorizationCrudHandler :
        AuthorizationHandler<OperationAuthorizationRequirement, UserWithEventsReadModel>
    {
        private readonly IUserService _userService;

        public UsersAuthorizationCrudHandler(IUserService userService)
        {
            _userService = userService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            UserWithEventsReadModel resource)
        {
            var user = await _userService.GetUserByClaims(context.User);

            if (user is null)
            {
                context.Fail();
            }

            var roleClaim = context.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .SingleOrDefault();

            if (requirement.Name == Operations.Update.Name
                || requirement.Name == Operations.Delete.Name)
            {
                if (resource.Email == user.Email)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    if (roleClaim is not null)
                    {
                        var roles = JsonConvert.DeserializeObject<IList<string>>(roleClaim.Value);

                        if (roles.Contains(MathEventRoles.Administrator))
                        {
                            context.Succeed(requirement);
                        }
                        else
                        {
                            context.Fail();
                        }
                    }
                    else
                    {
                        context.Fail();
                    }
                }
            }

            context.Succeed(requirement);
        }
    }
}
