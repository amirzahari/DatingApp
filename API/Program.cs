using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API
{
    public class Program
    {
        // remove the orginal code
        // public static void Main(string[] args)
        // {
        //     CreateHostBuilder(args).Build().Run();
        // }
        
        public static async Task Main(string[] args)
        {
            Console.WriteLine("==== building application main function ======= ");
            var host = CreateHostBuilder(args).Build();
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<DataContext>();
                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
                // *** automaticly set "dotnet ef database update" when running.
                await context.Database.MigrateAsync();
                // await Seed.SeedUsers(context);     
                await Seed.SeedUsers(userManager,roleManager);         
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex,"An error occurred during migrations");
            }
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
