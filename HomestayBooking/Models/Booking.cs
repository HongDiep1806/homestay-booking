namespace HomestayBooking.Models
{
    public class Booking
    {
        public int BookingID { get; set; }
        public int RoomQuantity { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; }
        public double TotalPrice { get; set; }
        public string CustomerId { get; set; }  
        public AppUser Customer { get; set; }   
        public string? StaffId { get; set; }     
        public AppUser? Staff { get; set; }
    }
}
