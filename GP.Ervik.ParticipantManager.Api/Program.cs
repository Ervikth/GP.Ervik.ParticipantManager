using GP.Ervik.ParticipantManager.Api.Exceptions;
using GP.Ervik.ParticipantManager.Api.Services;
using GP.Ervik.ParticipantManager.Api.Settings;
using GP.Ervik.ParticipantManager.Data;
using GP.Ervik.ParticipantManager.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

namespace GP.Ervik.ParticipantManager.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // This includes the connection string and database name required for setting up the MongoDB connection.
            var mongoDBSettings = builder.Configuration.GetSection("MongoDB").Get<MongoDbSettings>();

            // Validate MongoDB settings
            if (mongoDBSettings == null ||
                string.IsNullOrEmpty(mongoDBSettings.ConnectionString) ||
                string.IsNullOrEmpty(mongoDBSettings.DatabaseName))
            {
                throw new ConfigurationException("MongoDB configuration is missing or incomplete.");
            }

            // Register AutoMapper
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            // Add services to the container.
            builder.Services.AddDbContext<MongoDbContext>(options =>
            {
                options.UseMongoDB(mongoDBSettings.ConnectionString, mongoDBSettings.DatabaseName).EnableSensitiveDataLogging();
            });
            builder.Services.AddScoped<IAdministrationRepository, AdministrationRepository>();
            builder.Services.AddScoped<IParticipantRepository, ParticipantRepository>();

            builder.Services.AddScoped<AuthenticationService>(); // Register AuthenticationService

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            //Integrating authorization for swagger.
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "Remember write bearer first then key. Ex. bearer YOUR_JWT_TOKEN",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });
            //Jwt
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Key"])),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}