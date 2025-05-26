using HomestayBooking.Models;
using System.ComponentModel.DataAnnotations;

namespace HomestayBooking.DTOs.BookingDto
{
    public class CreateBookingDto
    {
        [Required]
        public DateTime CheckIn { get; set; }

        [Required]
        public DateTime CheckOut { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ít nhất một phòng.")]
        public List<int> SelectedRoomIds { get; set; } = new List<int>();

        public int Adults { get; set; }
        public int Children { get; set; }

        public List<Room>? AvailableRooms { get; set; }
    }
}
