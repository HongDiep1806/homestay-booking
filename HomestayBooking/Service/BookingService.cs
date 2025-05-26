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
        public async Task CreateBooking(Booking booking)
        {
            await _bookingRepository.CreateBooking(booking);
        }
    }
}
