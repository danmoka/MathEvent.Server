using MathEvent.Contracts.Services;
using MathEvent.Entities.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Threading.Tasks;

namespace MathEvent.AuthorizationHandlers.Organizations
{
    public class OrganizationsAuthorizationCrudHandler :
        AuthorizationHandler<OperationAuthorizationRequirement, Organization>
    {
        private readonly IUserService _userService;

        public OrganizationsAuthorizationCrudHandler(IUserService userService)
        {
            _userService = userService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            Organization resource)
        {
            var user = await _userService.GetUserAsync(context.User);

            if (requirement.Name == Operations.Update.Name
                || requirement.Name == Operations.Delete.Name)
            {
                if (resource.ManagerId == user.Id)
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
