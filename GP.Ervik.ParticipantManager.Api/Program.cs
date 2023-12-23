
using GP.Ervik.ParticipantManager.Api.Exceptions;
using GP.Ervik.ParticipantManager.Api.Settings;
using GP.Ervik.ParticipantManager.Data;
using Microsoft.EntityFrameworkCore;

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

            // Add services to the container.
            builder.Services.AddDbContext<MongoDbContext>(options =>
            {
                options.UseMongoDB(mongoDBSettings.ConnectionString, mongoDBSettings.DatabaseName);
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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