using HomestayBooking.Models;
using System.ComponentModel.DataAnnotations;

namespace HomestayBooking.DTOs.BookingDto
{
    public class CreateBookingDto
    {
        public int RoomTypeID { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [Range(1, 10)]
        public int RoomQuantity { get; set; }

        [Range(1, 10)]
        public int Adults { get; set; }

        [Range(0, 10)]
        public int Children { get; set; }
        [Required]
        public string UserId { get; set; }
        public string StaffId { get; set; } = null;
    }

}
