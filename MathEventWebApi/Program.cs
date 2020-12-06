using MathEventWebApi.Data.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace MathEventWebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                await Initialize(userManager);
            }

            host.Run();
            
        }

        public static async Task Initialize(UserManager<ApplicationUser> userManager)
        {
            const string testEmail = "test@mail.ru";
            const string testPassword = "!PPpassword12345";

            if (await userManager.FindByEmailAsync(testEmail) == null)
            {
                var test = new ApplicationUser { Email = testEmail, Name = "Test acc" };
                var result = await userManager.CreateAsync(test, testPassword);
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
