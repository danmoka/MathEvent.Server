using MathEvent.Contracts.Validators;
using MathEvent.Validation.Events;
using MathEvent.Validation.Files;
using MathEvent.Validation.Organization;
using MathEvent.Validation.Users;
using Microsoft.Extensions.DependencyInjection;

namespace MathEvent.Validation.Extensions
{
    /// <summary>
    /// Расширение IServiceCollection
    /// </summary>
    public static class ServiceExtension
    {
        /// <summary>
        /// Конфигурация валидации
        /// </summary>
        /// <param name="services">Сервисы</param>
        public static void ConfigureValidation(this IServiceCollection services)
        {
            services.AddTransient<IEventCreateModelValidator, EventCreateModelValidator>();
            services.AddTransient<IEventUpdateModelValidator, EventUpdateModelValidator>();
            services.AddTransient<IFileCreateModelValidator, FileCreateModelValidator>();
            services.AddTransient<IFileUpdateModelValidator, FileUpdateModelValidator>();
            services.AddTransient<IFileValidator, FileValidator>();
            services.AddTransient<IImageValidator, ImageValidator>();
            services.AddTransient<IOrganizationCreateModelValidator, OrganizationCreateModelValidator>();
            services.AddTransient<IOrganizationUpdateModelValidator, OrganizationUpdateModelValidator>();
            services.AddTransient<IUserCreateModelValidator, UserCreateModelValidator>();
            services.AddTransient<IUserUpdateModelValidator, UserUpdateModelValidator>();
            services.AddTransient<IForgotPasswordModelValidator, ForgotPasswordModelValidator>();
            services.AddTransient<IForgotPasswordResetModelValidator, ForgotPasswordResetModelValidator>();
            services.AddTransient<UserValidationUtils>();
            services.AddTransient<FileValidationUtils>();
            services.AddTransient<OrganizationValidationUtils>();
            services.AddTransient<EventValidationUtils>();
        }
    }
}
