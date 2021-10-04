using MathEvent.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace MathEvent.Repository.Extensions
{
    public static class ServiceExtension
    {
        /// <summary>
        /// Настройка репозитория
        /// </summary>
        /// <param name="services">Зависимости</param>
        public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        }
    }
}
