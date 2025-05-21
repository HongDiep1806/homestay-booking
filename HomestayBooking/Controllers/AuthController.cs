using AutoMapper;
using HomestayBooking.DTOs.AuthDto;
using HomestayBooking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HomestayBooking.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public AuthController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IMapper mapper)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || user.IsActive == false)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản không hợp lệ hoặc bị khoá.");
                return View(dto);
            }

            var result = await _signInManager.PasswordSignInAsync(user, dto.Password, dto.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Đăng nhập thất bại. Kiểm tra email hoặc mật khẩu.");
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Auth");
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterDto());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto dto)
        {

            var errors = new List<string>();

            var userExists = await _userManager.FindByEmailAsync(dto.Email);
            if (userExists != null)
            {
                errors.Add("Email đã tồn tại.");
                return View();
            }

            var user = _mapper.Map<AppUser>(dto);
            user.IdentityCard = "000000000";
            user.Gender = "Không xác định";
            user.DOB = new DateOnly(2000, 1, 1);
            user.Address = "Chưa cập nhật";

            user.IsActive = true;

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (result.Succeeded)
            {
                Console.WriteLine("tao thanh cong");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Login", "Auth");
            }
            foreach (var error in result.Errors)
            {
               errors.Add(error.Description);
                Console.WriteLine(error.Description);
            }
            if(errors.Count > 0)
            {
                return View();
            }
          
            Console.WriteLine($"FullName: {user.FullName}, Email: {user.Email}, UserName: {user.UserName}");

            return View(dto);
        }

    }
}
