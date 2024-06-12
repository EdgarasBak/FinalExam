using FinalExam.Database.Models;
using Microsoft.EntityFrameworkCore;


namespace FinalExam.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<PlaceOfResidence> PlaceOfResidences { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasMany(e => e.Persons)
                    .WithOne(p => p.User)
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Person>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.PlaceOfResidence)
                    .WithOne(p => p.Person)
                    .HasForeignKey<PlaceOfResidence>(p => p.PersonId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<PlaceOfResidence>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
        }

    }
}
