using MathEvent.Constants;
using MathEvent.Contracts;
using MathEvent.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MathEvent.AuthorizationHandlers.Files
{
    /// <summary>
    /// Обработчик запроса на авторизацию для CRUD операций файлов
    /// </summary>
    public class FilesAuthorizationCrudHandler :
        AuthorizationHandler<OperationAuthorizationRequirement, int>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;

        private readonly IUserService _userService;

        public FilesAuthorizationCrudHandler(
            IRepositoryWrapper repositoryWrapper,
            IUserService userService)
        {
            _repositoryWrapper = repositoryWrapper;
            _userService = userService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            int resource)
        {
            if (requirement.Name == Operations.Create.Name
               || requirement.Name == Operations.Update.Name
               || requirement.Name == Operations.Delete.Name)
            {
                var owner = await _repositoryWrapper
                    .Owner
                    .FindByCondition(ow => ow.Id == resource)
                    .SingleOrDefaultAsync();

                var user = await _userService.GetUserByClaims(context.User);

                if (owner.ApplicationUserId == user.Id)
                {
                    context.Succeed(requirement);
                    return;
                }

                if (owner.EventId is not null && user.ManagedEvents.Where(ev => ev.Id == (int)owner.EventId).Any())
                {
                    context.Succeed(requirement);
                    return;
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
