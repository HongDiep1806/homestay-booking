using HomestayBooking.DTOs.BookingDto;
using HomestayBooking.Models;
using HomestayBooking.Repositories;

namespace HomestayBooking.Service
{
    public class BookingService : IBookingService

    {
        private readonly IBookingRepository _bookingRepository;
        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }
        public async Task<bool> CreateBooking(CreateBookingDto dto)
        {
            return await _bookingRepository.CreateBooking(dto);
        }

        public async Task<List<int>> GetAvailableRoomTypeIdsAsync(DateTime checkIn, DateTime checkOut, int adults, int children, int roomQuantity)
        {
            return await _bookingRepository.GetAvailableRoomTypeIdsAsync(checkIn, checkOut, adults, children, roomQuantity);
        }
    }
}
