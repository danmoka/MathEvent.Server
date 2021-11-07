using MathEvent.Api.Configuration;
using MathEvent.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;

namespace MathEvent.Api.Extensions
{
    /// <summary>
    /// Статический класс, расширяющий IServiceCollection
    /// </summary>
    public static class ServiceExtension
    {
        /// <summary>
        /// Настройка аутентификации
        /// </summary>
        /// <param name="services">Зависимости</param>
        /// <param name="configuration">Поставщик конфигурации</param>
        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var authenticationSettings = configuration.GetSection("Authentication");
            services.AddAuthentication().AddJwtBearer(options =>
            {
                options.Authority = authenticationSettings["Authority"];
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = true,
                    ValidIssuer = authenticationSettings["IssuerUri"],
                };
            });
        }

        /// <summary>
        /// Настройка авторизации
        /// </summary>
        /// <param name="services">Зависимости</param>
        /// <param name="configuration">Поставщик конфигурации</param>
        public static void ConfigureAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            var authenticationSettings = configuration.GetSection("Authentication");

            services.AddAuthorization(options =>
            {
                options.AddPolicy("MathEventApi", policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", authenticationSettings
                        .GetSection("Scopes")
                        .Get<string[]>());
                });
            });
        }

        /// <summary>
        /// Настройка контроллеров и json
        /// </summary>
        /// <param name="services">Зависимости</param>
        public static void ConfigureControllers(this IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });
        }

        /// <summary>
        /// Настройка отправки email сообщений
        /// </summary>
        /// <param name="services">Зависимости</param>
        /// <param name="configuration">Поставщик конфигурации</param>
        public static void ConfigureEmail(this IServiceCollection services, IConfiguration configuration)
        {
            var emailConfig = configuration
                .GetSection("Email")
                .Get<EmailConfiguration>();
            services.AddSingleton<IEmailConfiguration>(emailConfig);
        }

        /// <summary>
        /// Настройка OpenApi
        /// </summary>
        /// <param name="services">Зависимости</param>
        public static void ConfigureOpenApi(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MathEvent.Api", Version = "v1" });
            });
        }
    }
}
