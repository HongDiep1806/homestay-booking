using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HomestayBooking.Models.DAL
{
    public class AppDbContext: IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Customer> Customers { get; set; }  
        public DbSet<Booking> Bookings { get; set; }    
        public DbSet<Booking_Room> Booking_Rooms { get; set; }
        public DbSet<Role> Roles {  get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Staff> Staffs { get; set; }



    }
}
