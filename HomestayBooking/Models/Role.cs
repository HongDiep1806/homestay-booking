namespace HomestayBooking.Models
{
    public class Role
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public ICollection<Staff> Staffs { get; set; }
    }
}
