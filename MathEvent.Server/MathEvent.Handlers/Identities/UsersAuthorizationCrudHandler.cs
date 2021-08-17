using MathEvent.Entities.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace MathEvent.AuthorizationHandlers.Identities
{
    /// <summary>
    /// Обработчик запроса на авторизацию для CRUD операций пользователей
    /// </summary>
    public class UsersAuthorizationCrudHandler :
        AuthorizationHandler<OperationAuthorizationRequirement, ApplicationUser>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersAuthorizationCrudHandler(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            ApplicationUser resource)
        {
            var user = await _userManager.GetUserAsync(context.User);

            if (requirement.Name == Operations.Update.Name
                || requirement.Name == Operations.Delete.Name)
            {
                if (resource.Id == user.Id)
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
