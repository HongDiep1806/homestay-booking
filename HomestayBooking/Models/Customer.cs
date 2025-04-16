namespace HomestayBooking.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string IdentityCard { get; set; }
        public string Gender { get; set; }
        public DateOnly DOB { get; set; }
        public string HashPassword { get; set; }
        public ICollection<Booking> Bookings { get; set; }

    }
}
