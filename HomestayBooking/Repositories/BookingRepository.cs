using HomestayBooking.Models;
using HomestayBooking.Models.DAL;

namespace HomestayBooking.Repositories
{
    public class BookingRepository: BaseRepository<Booking>, IBookingRepository
    {

        public BookingRepository(AppDbContext context) : base(context)
        {
        }

        public async Task CreateBooking(Booking booking)
        {
           await Create(booking);
        }
    }
 
}
