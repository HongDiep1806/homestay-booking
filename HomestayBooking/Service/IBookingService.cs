using HomestayBooking.DTOs.BookingDto;
using HomestayBooking.Models;

namespace HomestayBooking.Service
{
    public interface IBookingService
    {
        Task<bool> CreateBooking(CreateBookingDto dto);
    }
}
