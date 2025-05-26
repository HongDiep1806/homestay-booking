namespace HomestayBooking.Models
{
    public class Booking: BaseEntity
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
        public bool IsDeleted { get; set; } = false;
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }

    }
}
