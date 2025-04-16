using Microsoft.AspNetCore.Identity;

namespace HomestayBooking.Models
{
    public class Staff
    {
        public int StaffID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string IdentityCard { get; set; }
        public string Gender { get; set; }
        public DateOnly DOB { get; set; }
        public string HashPassword { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public int RoleID { get; set; }
        public Role Role { get; set; }
        
    }
}
