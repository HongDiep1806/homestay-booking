namespace HomestayBooking.DTOs.UserDto
{
    public class UserDto
    {
        public string FullName { get; set; }
        public string IdentityCard { get; set; }
        public string Gender { get; set; }
        public DateOnly DOB { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
    }
}
