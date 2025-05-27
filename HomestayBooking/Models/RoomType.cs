namespace HomestayBooking.Models
{
    public class RoomType: BaseEntity
    {
        public int RoomTypeID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Capacity { get; set; }
        public double Price { get; set; }
        public ICollection<Room> Rooms { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
