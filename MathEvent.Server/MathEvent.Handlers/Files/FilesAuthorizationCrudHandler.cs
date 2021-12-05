using MathEvent.Contracts;
using MathEvent.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
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
            var user = await _userService.GetUserByClaims(context.User);

            if (user is null)
            {
                context.Fail();
            }

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
                    if (user.ManagedEvents.Where(ev => ev.Id == (int)owner.EventId).Any())
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
