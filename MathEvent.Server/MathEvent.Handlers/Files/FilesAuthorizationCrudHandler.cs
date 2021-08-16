using MathEvent.Contracts;
using MathEvent.Entities.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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

        private readonly UserManager<ApplicationUser> _userManager;

        public FilesAuthorizationCrudHandler(
            IRepositoryWrapper repositoryWrapper,
            UserManager<ApplicationUser> userManager)
        {
            _repositoryWrapper = repositoryWrapper;
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            int resource)
        {
            var user = await _userManager.GetUserAsync(context.User);
            var owner = await _repositoryWrapper
                .Owner
                .FindByCondition(ow => ow.Id == resource)
                .SingleOrDefaultAsync();

            if (requirement.Name == Operations.Create.Name
                || requirement.Name == Operations.Update.Name
                || requirement.Name == Operations.Delete.Name)
            {
                if (owner.ApplicationUserId == user.Id)
                {
                    context.Succeed(requirement);
                }
                else if (owner.EventId is not null)
                {
                    var userManagedEventIds = await _repositoryWrapper
                        .Management
                        .FindByCondition(m => m.ApplicationUserId == user.Id)
                        .Select(m => m.EventId)
                        .ToListAsync();

                    if (userManagedEventIds.Contains((int)owner.EventId))
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

            context.Succeed(requirement);
        }
    }
}
