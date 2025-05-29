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
    }
}
