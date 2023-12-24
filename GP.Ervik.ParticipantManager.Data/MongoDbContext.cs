using GP.Ervik.ParticipantManager.Data.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace GP.Ervik.ParticipantManager.Data
{
    public class MongoDbContext : DbContext
    {
        public DbSet<Administration>? Administrations { get; set; }
        public DbSet<Participant>? Participants { get; set; }
        public MongoDbContext(DbContextOptions<MongoDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Participant>(u =>
            {
                u.ToCollection("participant");
                u.HasKey(v => v.Id);
                u.Property(v => v.Name).HasElementName("name");
                u.Property(v => v.Email).HasElementName("email");
                u.Property(v => v.PhoneNumber).HasElementName("phonenumber");
                u.Property(v => v.Allergens).HasElementName("allergens");
                u.Property(v => v.Comment).HasElementName("comment");
            });
            modelBuilder.Entity<Administration>(u =>
            {
                u.ToCollection("administration");
                u.HasKey(v => v.Id);
                u.Property(v => v.Name).HasElementName("name");
                u.Property(v => v.Email).HasElementName("email");
                u.Property(v => v.Password).HasElementName("password");
            });
        }
    }
}
