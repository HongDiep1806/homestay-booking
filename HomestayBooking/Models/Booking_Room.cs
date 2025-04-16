namespace HomestayBooking.Models
{
    public class Booking_Room
    {
        public int Booking_RoomID { get; set; }
        public int RoomID { get; set; }
        public Room Room { get; set; }  
        public int BookingID {  get; set; }
        public Booking Booking { get; set; }    
        
    }
}
