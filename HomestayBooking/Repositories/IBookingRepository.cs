using HomestayBooking.Models;

namespace HomestayBooking.Repositories
{
    public interface IBookingRepository: IBaseRepository<Booking>
    {
        Task CreateBooking(Booking booking);
        Task<List<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, int adults, int children);

    }
}
