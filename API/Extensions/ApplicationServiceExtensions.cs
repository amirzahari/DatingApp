using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services, IConfiguration config)
        {
            // *** [aznote] Add Interface Implementation for token.
            services.AddScoped<ITokenService, TokenService>();
            // *** [aznote] Add Interface Implementation for User Repository
            services.AddScoped<IUserRepository, UserRepository>();
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