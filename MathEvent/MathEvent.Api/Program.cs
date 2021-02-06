using MathEvent.Entities;
using MathEvent.Entities.Models.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace MathEvent.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var context = serviceProvider.GetRequiredService<RepositoryContext>();
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                //await InitializeAdminAccount(userManager);
            }

            host.Run();
        }

        private static async Task InitializeAdminAccount(UserManager<ApplicationUser> userManager)
        {
            const string adminEmail = "";
            const string adminPassword = "";

            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser { Email = adminEmail, UserName = adminEmail };
                var result = await userManager.CreateAsync(admin, adminPassword);
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
