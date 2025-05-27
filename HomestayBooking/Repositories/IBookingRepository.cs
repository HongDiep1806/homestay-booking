using HomestayBooking.DTOs.BookingDto;
using HomestayBooking.Models;
using Microsoft.AspNetCore.Mvc;

namespace HomestayBooking.Repositories
{
    public interface IBookingRepository: IBaseRepository<Booking>
    {
        Task CreateBooking(Booking booking);
        Task<List<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, int adults, int childrens);
        Task<bool> CreateBooking(CreateBookingDto dto);

    }
}
