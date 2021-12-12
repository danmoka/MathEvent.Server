using MathEvent.Api.Extensions;
using MathEvent.AuthorizationHandlers.Extension;
using MathEvent.Converters.Extension;
using MathEvent.Database.Extensions;
using MathEvent.Email.Extensions;
using MathEvent.Repository.Extensions;
using MathEvent.ScheduledJobs.Extensions;
using MathEvent.Services.Extension;
using MathEvent.Validation.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MathEvent.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureConnection(Configuration);
            services.ConfigureAuthentication(Configuration);
            services.ConfigureAuthorization(Configuration);
            services.ConfigureRepositoryWrapper();
            services.ConfigureEmail();
            services.ConfigureEntityServices();
            services.ConfigureAuthorizationHandlers();
            services.ConfigureMapper();
            services.ConfigureValidation();
            services.ConfigureControllers();
            services.ConfigureOpenApi();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MathEvent.Api v1"));
            }

            app.UseCors(builder =>
            {
                builder.WithOrigins(Configuration.GetSection("Origins").Get<string[]>());
                builder.AllowCredentials();
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.WithExposedHeaders("Content-Disposition");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers()
                .RequireAuthorization("MathEventApi");
            });
        }
    }
}