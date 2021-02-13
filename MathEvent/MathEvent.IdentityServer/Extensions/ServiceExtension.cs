using IdentityServer;
using MathEvent.Entities;
using MathEvent.Entities.Models.Identities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MathEvent.IdentityServer.Extensions
{
    /// <summary>
    /// Статический класс, расширяющий IServiceCollection
    /// </summary>
    public static class ServiceExtension
    {
        /// <summary>
        /// Настройка подключения к базе данных
        /// </summary>
        /// <param name="services">Зависимости</param>
        /// <param name="configuration">Поставщик конфигурации</param>
        public static void ConfigureConnection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<RepositoryContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DBConnection"));
            });
        }

        /// <summary>
        /// Настройка IdentityServer4
        /// </summary>
        /// <param name="services">Зависимости</param>
        public static void ConfigureIdentityServer(this IServiceCollection services)
        {
            services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.EmitStaticAudienceClaim = true;
            })
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.Clients)
                .AddAspNetIdentity<ApplicationUser>()
                .AddDeveloperSigningCredential(); // not recommended for production - you need to store your key material somewhere secure
        }
    }
}
