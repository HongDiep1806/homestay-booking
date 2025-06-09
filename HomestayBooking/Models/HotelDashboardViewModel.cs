namespace HomestayBooking.Models
{
    public class RoomTypeRatioItem
    {
        public string RoomTypeName { get; set; }
        public int Count { get; set; }
    }

    public class HotelDashboardViewModel
    {
        public int TotalBookings { get; set; }
        public int AvailableRooms { get; set; }
        public List<RoomTypeRatioItem> RoomTypeRatios { get; set; }
        public List<Booking> RecentBookings { get; set; }
    }


}
