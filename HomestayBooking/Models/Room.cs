namespace HomestayBooking.Models
{
    public class Room
    {
        public int RoomID   { get; set; }
        public string RoomCode { get; set; }
        public bool RoomStatus { get; set; }
        public int RoomTypeID { get; set; }
        public RoomType RoomType { get; set; }
        
        public string RoomImg { get; set; }
    }
}
