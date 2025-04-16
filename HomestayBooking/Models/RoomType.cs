namespace HomestayBooking.Models
{
    public class RoomType
    {
        public int RoomTypeID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int RoomQuantity { get; set; }
        public double Price { get; set; }
        public ICollection<Room> Rooms { get; set; }        
    }
}
