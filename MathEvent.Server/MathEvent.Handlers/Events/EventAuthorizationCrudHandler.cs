using MathEvent.Contracts;
using MathEvent.Entities.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace MathEvent.Handlers.Events
{
    /// <summary>
    /// Обработчик запроса на авторизацию для CRUD операций событий
    /// </summary>
    public class EventAuthorizationCrudHandler :
        AuthorizationHandler<OperationAuthorizationRequirement, Event>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;

        private readonly UserManager<ApplicationUser> _userManager;

        public EventAuthorizationCrudHandler(
            IRepositoryWrapper repositoryWrapper,
            UserManager<ApplicationUser> userManager)
        {
            _repositoryWrapper = repositoryWrapper;
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            Event resource)
        {
            var user = await _userManager.GetUserAsync(context.User);
            var userManagedEventIds = await _repositoryWrapper
                .Management
                .FindByCondition(m => m.ApplicationUserId == user.Id)
                .Select(m => m.EventId)
                .ToListAsync();

            if (requirement.Name == Operations.Update.Name
                || requirement.Name == Operations.Delete.Name)
            {
                if (userManagedEventIds.Contains(resource.Id))
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
