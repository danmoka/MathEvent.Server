using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MathEvent.Database.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureConnection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<RepositoryContext>(options => options.UseSqlServer(
                configuration.GetConnectionString("DBConnection")));
        }
    }
}
