using MathEvent.AuthorizationHandlers.Events;
using MathEvent.AuthorizationHandlers.Files;
using MathEvent.AuthorizationHandlers.Identities;
using MathEvent.AuthorizationHandlers.Organizations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace MathEvent.AuthorizationHandlers.Extension
{
    public static class ServiceExtension
    {
        /// <summary>
        /// Настройка обработчиков авторизации
        /// </summary>
        /// <param name="services">Зависимости</param>
        public static void ConfigureAuthorizationHandlers(this IServiceCollection services)
        {
            services.AddScoped<IAuthorizationHandler, EventAuthorizationCrudHandler>();
            services.AddScoped<IAuthorizationHandler, FilesAuthorizationCrudHandler>();
            services.AddScoped<IAuthorizationHandler, OrganizationsAuthorizationCrudHandler>();
            services.AddScoped<IAuthorizationHandler, UsersAuthorizationCrudHandler>();
        }
    }
}
