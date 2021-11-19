using MathEvent.Contracts.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MathEvent.Email.Extensions
{
    public static class ServiceExtension
    {
        public static void ConfigureEmail(this IServiceCollection services)
        {
            services.AddTransient<IEmailService, EmailService>();
        }
    }
}
