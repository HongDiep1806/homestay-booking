using HomestayBooking.Models;

namespace HomestayBooking.Service
{
    public interface IBookingService
    {
        Task CreateBooking(Booking booking);
    }
}
