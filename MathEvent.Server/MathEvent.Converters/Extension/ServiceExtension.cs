using AutoMapper;
using MathEvent.Converters.Events.Profiles;
using MathEvent.Converters.Identities.Profiles;
using Microsoft.Extensions.DependencyInjection;

namespace MathEvent.Converters.Extension
{
    public static class ServiceExtension
    {
        /// <summary>
        /// Настройка маппера
        /// </summary>
        /// <param name="services">Зависимости</param>
        public static void ConfigureMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(UserProfile), typeof(EventProfile));
        }
    }
}
