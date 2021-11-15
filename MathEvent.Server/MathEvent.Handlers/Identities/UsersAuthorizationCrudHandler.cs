using MathEvent.Contracts.Services;
using MathEvent.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
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
            var user = await _userService.GetUserAsync(context.User);

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
