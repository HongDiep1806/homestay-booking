namespace HomestayBooking.Models
{
    public class Booking : BaseEntity
    {
        public int BookingID { get; set; }

        public int RoomQuantity { get; set; } 
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public double TotalPrice { get; set; } 

        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }

        public string CustomerId { get; set; }
        public AppUser Customer { get; set; }

        public string? StaffId { get; set; }
        public AppUser? Staff { get; set; }

        public int RoomTypeID { get; set; }
        public RoomType RoomType { get; set; }

        public ICollection<Booking_Room> Booking_Rooms { get; set; } = new List<Booking_Room>();

        public bool IsDeleted { get; set; } = false;
    }

}
