using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HomestayBooking.Models.DAL
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Booking_Room> Booking_Rooms { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Customer)
                .WithMany()
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Staff)
                .WithMany()
                .HasForeignKey(b => b.StaffId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Room>()
    .HasOne(r => r.RoomType)
    .WithMany(rt => rt.Rooms)
    .HasForeignKey(r => r.RoomTypeID)
    .OnDelete(DeleteBehavior.Restrict);

        }

    }
}
