using HomestayBooking.DTOs.BookingDto;

namespace HomestayBooking.Models
{
    public class RoomsViewModel
    {
        public List<RoomType> RoomTypes { get; set; }
        public CreateBookingDto BookingInfo { get; set; }
    }


}
