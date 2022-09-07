
var builder = WebApplication.CreateBuilder(args);

// add service to the container

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddCors();
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddSignalR();

// configure the HTTP request pipeline

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

//app.UseRouting();

// *** [aznote] Add for CORS request blocked. (specific allow URL)
app.UseCors(x => x.AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials() // *** for signalR
    .WithOrigins("https://localhost:4200"));

// *** [aznote] Add for recognize Bearer Authorization Token
app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");
app.MapFallbackToController("Index", "Fallback");



AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
//var host = CreateHostBuilder(args).Build();
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    // *** automaticly set "dotnet ef database update" when running.
    await context.Database.MigrateAsync();
    // await Seed.SeedUsers(context);     
    await Seed.SeedUsers(userManager, roleManager);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migrations");
}

await app.RunAsync();


// app.UseEndpoints(endpoints =>
// {
//     endpoints.MapControllers();
//     endpoints.MapHub<PresenceHub>("hubs/presence");
//     endpoints.MapHub<MessageHub>("hubs/message");
//     endpoints.MapFallbackToController("Index", "Fallback");
// });

// namespace API
// {
//     public class Prograbuilder.S
//     {
//         // remove the orginal code
//         // public static void Main(string[] args)
//         // {
//         //     CreateHostBuilder(args).Build().Run();
//         // }

//         public static async Task Main(string[] args)
//         {
//             AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
//             var host = CreateHostBuilder(args).Build();
//             using var scope = host.Services.CreateScope();
//             var services = scope.ServiceProvider;
//             try
//             {
//                 var context = services.GetRequiredService<DataContext>();
//                 var userManager = services.GetRequiredService<UserManager<AppUser>>();
//                 var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
//                 // *** automaticly set "dotnet ef database update" when running.
//                 await context.Database.MigrateAsync();
//                 // await Seed.SeedUsers(context);     
//                 await Seed.SeedUsers(userManager,roleManager);         
//             }
//             catch (Exception ex)
//             {
//                 var logger = services.GetRequiredService<ILogger<Program>>();
//                 logger.LogError(ex,"An error occurred during migrations");
//             }
//             await host.RunAsync();
//         }

//         public static IHostBuilder CreateHostBuilder(string[] args) =>
//             Host.CreateDefaultBuilder(args)
//                 .ConfigureWebHostDefaults(webBuilder =>
//                 {
//                     webBuilder.UseStartup<Startup>();
//                 });
//     }
// }
