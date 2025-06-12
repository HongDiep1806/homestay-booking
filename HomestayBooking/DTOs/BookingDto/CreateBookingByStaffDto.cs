namespace HomestayBooking.DTOs.BookingDto
{
    public class CreateBookingByStaffDto
    {
        public string CustomerId { get; set; }
        public int RoomTypeID { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int RoomQuantity { get; set; }
    }
}
