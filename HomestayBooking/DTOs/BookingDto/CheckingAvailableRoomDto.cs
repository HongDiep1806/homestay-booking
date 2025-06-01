using System.ComponentModel.DataAnnotations;

namespace HomestayBooking.DTOs.BookingDto
{
    public class CheckingAvailableRoomDto
    {
        [Required]
        public DateTime CheckIn { get; set; }

        [Required]
        public DateTime CheckOut { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public int RoomQuantity { get; set; } 

    }
}
