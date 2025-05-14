using HomestayBooking.Models;

namespace HomestayBooking.DTOs.RoomDto
{
    public class RoomDto
    {
        public int RoomID { get; set; }
        public string RoomCode { get; set; }
        public bool RoomStatus { get; set; }
        public int RoomTypeID { get; set; }
        public string RoomType { get; set; }
        public string RoomImg { get; set; }
    }
}
