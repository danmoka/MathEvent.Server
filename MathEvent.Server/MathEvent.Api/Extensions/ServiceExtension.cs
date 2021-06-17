using AutoMapper;
using MathEvent.Contracts;
using MathEvent.Converters.Events.Profiles;
using MathEvent.Converters.Identities.Profiles;
using MathEvent.Entities;
using MathEvent.Repository;
using MathEvent.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;

namespace MathEvent.Api.Extensions
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
        /// Настройка аутентификации
        /// </summary>
        /// <param name="services">Зависимости</param>
        /// <param name="configuration">Поставщик конфигурации</param>
        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // TODO: взять ссылку из конфигурацию
            // accepts any access token issued by identity server
            services.AddAuthentication().AddJwtBearer(options =>
            {
                options.Authority = configuration.GetValue<string>("Authority");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false
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
            // TODO: использовать configuration для начитки строковых значений
            // adds an authorization policy to make sure the token is for scope 'matheventapi'
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", new string[] { "matheventapi" });
                });
            });
        }

        /// <summary>
        /// Настройка репозитория
        /// </summary>
        /// <param name="services">Зависимости</param>
        public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        }

        public static void ConfigureEntityServices(this IServiceCollection services, IWebHostEnvironment env, IConfiguration configuration)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IOwnerService, OwnerService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IOrganizationService, OrganizationService>();
            services.AddSingleton(new DataPathService(env.WebRootPath, configuration.GetValue<long>("FileSizeLimit")));
        }

        /// <summary>
        /// Настройка маппера
        /// </summary>
        /// <param name="services">Зависимости</param>
        public static void ConfigureMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(UserProfile), typeof(EventProfile));
        }

        /// <summary>
        /// Настройка json
        /// </summary>
        /// <param name="services">Зависимости</param>
        public static void ConfigureJson(this IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });
        }
    }
}
