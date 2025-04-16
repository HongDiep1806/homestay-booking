namespace HomestayBooking.Models
{
    public class Booking
    {
        public int BookingID { get; set; }
        public int RoomQuantity { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; }
        public double TotalPrice { get; set; }
        public int CustomerID { get; set; }
        public Customer Customer { get; set; }
        public int StaffID { get; set; }
        public Staff Staff { get; set; }
    }
}
