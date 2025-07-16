using InternalBookingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace InternalBookingApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<ResourceManagement> ResourceManagements { get; set; }

        public DbSet<BookingModel> BookingModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ResourceManagement>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Description).HasMaxLength(750);
                entity.Property(e => e.Location).HasMaxLength(150);
                entity.Property(e => e.Capacity).IsRequired();
                entity.Property(e => e.IsAvailable).IsRequired();

            });

            modelBuilder.Entity<BookingModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ResourceId).IsRequired();
                entity.Property(e => e.StartTime).IsRequired();
                entity.Property(e => e.EndTime).IsRequired();
                entity.Property(e => e.BookedBy).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Purpose).IsRequired().HasMaxLength(150);

                // Configuring relationship models
                /*entity.HasOne(e => e.ResourceManagement)
                      .WithMany(r => r.BookingModels)
                      .HasForeignKey(e => e.ResourceId)
                      .OnDelete(DeleteBehavior.Cascade);*/

                modelBuilder.Entity<BookingModel>()
                .HasOne(b => b.ResourceManagement)
                .WithMany(r => r.BookingModels)
                .HasForeignKey(b => b.ResourceId)
                .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }



}

