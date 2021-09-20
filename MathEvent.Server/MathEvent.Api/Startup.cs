using MathEvent.Api.Extensions;
using MathEvent.Database.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScheduledJobs.Extensions;

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
            services.ConfigureIndentity();
            services.ConfigureAuthentication(Configuration);
            services.ConfigureAuthorization();
            services.ConfigureRepositoryWrapper();
            services.ConfigureEmail(Configuration);
            services.ConfigureEntityServices(Environment, Configuration);
            services.ConfigureAuthorizationHandlers();
            services.ConfigureMapper();
            services.ConfigureJson();
            services.ConfigureQuartz();
            services.AddSwaggerGen();
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
                .RequireAuthorization("ApiScope");
            });
        }
    }
}