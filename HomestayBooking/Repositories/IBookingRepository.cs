using HomestayBooking.Models;

namespace HomestayBooking.Repositories
{
    public interface IBookingRepository: IBaseRepository<Booking>
    {
        Task CreateBooking(Booking booking);
    }
}
