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
        public async Task<List<Booking>> GetBookingAsync()
        {
            return await _bookingRepository.GetAllBooking();
        }

        public async Task<List<Booking>> GetInvoiceAsync()
        {
            return await _bookingRepository.GetInvoice();
        }
        public async Task<bool> CreateBookingByStaffAsync(CreateBookingByStaffDto dto)
        {
            return await _bookingRepository.CreateBookingByStaffAsync(dto);
        }
        public async Task<Booking> GetBookingByIdAsync(int bookingId)
        {
            return await _bookingRepository.GetById(bookingId);
        }
        public async Task<bool> UpdateBookingAsync(int bookingId, Booking updatedBooking)
        {
            var existingBooking = await _bookingRepository.GetById(bookingId);
            if (existingBooking == null)
            {
                return false; 
            }
            await _bookingRepository.GetAvailableRoomTypeIdsAsync(updatedBooking.CheckIn, updatedBooking.CheckOut, 1, 0, updatedBooking.RoomQuantity);
            existingBooking.CheckIn = updatedBooking.CheckIn;
            existingBooking.CheckOut = updatedBooking.CheckOut;
            existingBooking.RoomQuantity = updatedBooking.RoomQuantity;
            existingBooking.Status = updatedBooking.Status;
            existingBooking.TotalPrice = updatedBooking.RoomType.Price * updatedBooking.RoomQuantity;
            await _bookingRepository.Update(bookingId, existingBooking);
            return true;
        }
    }
}
