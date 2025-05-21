using HomestayBooking.Models;
using Microsoft.AspNetCore.Identity;

namespace HomestayBooking.Models
{
    public class AppUser: IdentityUser, BaseEntity
    {
        public string FullName { get; set; }
        public string IdentityCard { get; set; }
        public string Gender { get; set; }
        public DateOnly DOB { get; set; }
        public string Address { get; set; }
        public bool? IsActive { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
