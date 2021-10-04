using MathEvent.Contracts.Services;
using MathEvent.Services.Services;
using MathEvent.Services.Services.DataPath;
using MathEvent.Services.Services.Email;
using Microsoft.Extensions.DependencyInjection;

namespace MathEvent.Services.Extension
{
    public static class ServiceExtension
    {
        public static void ConfigureEntityServices(this IServiceCollection services, string webRootPath, long fileSizeLimit)
        {
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IEventService, EventService>();
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<IOrganizationService, OrganizationService>();
            services.AddTransient<IDataPathWorker>(s => new DataPathWorker(webRootPath, fileSizeLimit, new FileExtensionWorker()));
            services.AddTransient<IEmailSender, EmailSender>();
        }
    }
}
