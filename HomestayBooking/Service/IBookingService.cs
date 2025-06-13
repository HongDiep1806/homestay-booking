using HomestayBooking.DTOs.BookingDto;
using HomestayBooking.Models;

namespace HomestayBooking.Service
{
    public interface IBookingService
    {
        Task<bool> CreateBooking(CreateBookingDto dto);
        Task<bool> CheckAvailability(CreateBookingDto dto);

        Task<List<int>> GetAvailableRoomTypeIdsAsync(DateTime checkIn, DateTime checkOut, int adults, int children, int roomQuantity);
        Task<List<Booking>> GetBookingAsync();
        Task<List<Booking>> GetInvoiceAsync();
        Task<bool> CreateBookingByStaffAsync(CreateBookingByStaffDto dto);
        Task<Booking> GetBookingByIdAsync(int bookingId);
        Task<bool> UpdateBookingAsync(int bookingId, Booking updatedBooking);
    }
}
