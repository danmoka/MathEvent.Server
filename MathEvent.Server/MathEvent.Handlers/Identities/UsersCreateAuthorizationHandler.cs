using MathEvent.Constants;
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
    /// Обработчик запроса на авторизацию для создания пользователей
    /// </summary>
    public class UsersCreateAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, UserCreateModel>
    {

        public UsersCreateAuthorizationHandler()
        {
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            UserCreateModel resource)
        {
            var roleClaim = context.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .SingleOrDefault();
            var emailClaim = context.User.Claims
                .Where(c => c.Type == ClaimTypes.Email)
                .SingleOrDefault();

            if (requirement.Name == Operations.Create.Name)
            {
                if (resource.Email == emailClaim.Value)
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
