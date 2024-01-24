using GP.Ervik.ParticipantManager.Api.Exceptions;
using GP.Ervik.ParticipantManager.Api.Interfaces;
using GP.Ervik.ParticipantManager.Api.Services;
using GP.Ervik.ParticipantManager.Api.Settings;
using GP.Ervik.ParticipantManager.Data;
using GP.Ervik.ParticipantManager.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GP.Ervik.ParticipantManager.Api.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // This includes the connection string and database name required for setting up the MongoDB connection.
            var mongoDBSettings = config.GetSection("MongoDB").Get<MongoDbSettings>();

            // Validate MongoDB settings
            if (mongoDBSettings == null ||
                string.IsNullOrEmpty(mongoDBSettings.ConnectionString) ||
                string.IsNullOrEmpty(mongoDBSettings.DatabaseName))
            {
                throw new ConfigurationException("MongoDB configuration is missing or incomplete.");
            }
            services.AddDbContext<MongoDbContext>(options =>
            {
                options.UseMongoDB(mongoDBSettings.ConnectionString, mongoDBSettings.DatabaseName).EnableSensitiveDataLogging();
            });

            // Register AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            // Add services to the container.

            services.AddScoped<IAdministrationRepository, AdministrationRepository>();
            services.AddScoped<IParticipantRepository, ParticipantRepository>();

            services.AddScoped<ITokenService, TokenService>();// Register AuthenticationService

            return services;
        }
    }
}
