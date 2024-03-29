﻿using MathEvent.Contracts.Services;
using MathEvent.Services.Services;
using MathEvent.Services.Services.DataPath;
using Microsoft.Extensions.DependencyInjection;

namespace MathEvent.Services.Extension
{
    public static class ServiceExtension
    {
        public static void ConfigureEntityServices(this IServiceCollection services)
        {
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IEventService, EventService>();
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<IOrganizationService, OrganizationService>();
            services.AddTransient<IFileExtensionWorker, FileExtensionWorker>();
            services.AddTransient<IDataPathWorker, DataPathWorker>();
        }
    }
}
