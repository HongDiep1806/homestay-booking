using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace HomestayBooking.DTOs.AuthDto
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Mật khẩu ít nhất 6 ký tự.")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; }
    }
}
