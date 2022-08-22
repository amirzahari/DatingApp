using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SignalR;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<PresenceTracker>();

            // *** [aznote] Cloudinary Setting
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

            // *** [aznote] Add Interface Implementation
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ILikesRepository, LikesRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();

            // *** Handle process before execute httpcontext.
            services.AddScoped<LogUserActivity>();

            // *** [aznote] Add automapper implementation   
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            
            // *** [aznote] Add DB context.
            services.AddDbContext<DataContext>(option =>
            {
                option.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            return services;
        }
    }
}