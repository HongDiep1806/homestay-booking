using HomestayBooking.DTOs.BookingDto;
using HomestayBooking.Models;
using Microsoft.AspNetCore.Mvc;

namespace HomestayBooking.Repositories
{
    public interface IBookingRepository : IBaseRepository<Booking>
    {
        //Task CreateBooking(Booking booking);
        Task<List<int>> GetAvailableRoomTypeIdsAsync(DateTime checkIn, DateTime checkOut, int adults, int childrens, int roomQuantity);
        Task<bool> CreateBooking(CreateBookingDto dto);
        Task<List<Booking>> GetAllBooking();
        Task<List<Booking>> GetInvoice();
        Task<bool> CreateBookingByStaffAsync(CreateBookingByStaffDto dto);
        Task<bool> CheckAvailability(CreateBookingDto dto);
        Task<List<Booking>> GetMyBookingsAsync(string userId);


    }
}
