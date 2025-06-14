using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;
using AutoMapper;
using HomestayBooking.DTOs.BookingDto;
using HomestayBooking.DTOs.UserDto;
using HomestayBooking.Models;
using HomestayBooking.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HomestayBooking.Controllers
{
    [Authorize(Roles = "Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRoomService _roomService;
        private readonly IBookingService _bookingService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IRoomTypeService _roomTypeService;
        private readonly IMapper _mapper;
        private readonly SignInManager<AppUser> _signInManager;


        public HomeController(ILogger<HomeController> logger, IRoomTypeService roomTypeService, IRoomService roomService, IBookingService bookingService, UserManager<AppUser> userManager, IMapper mapper, SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _roomService = roomService;
            _bookingService = bookingService;
            _userManager = userManager;
            _mapper = mapper;
            _roomTypeService = roomTypeService;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            var model = new CheckingAvailableRoomDto
            {
                CheckIn = DateTime.Today.AddDays(1).Date.AddHours(12), 
                CheckOut = DateTime.Today.AddDays(2).Date.AddHours(12),
                Adults = 1,
                Children = 0,
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult NotFound404()
        {
            return View();
        }
        //public async Task<IActionResult> Rooms(string idsJson, string checkIn, string checkOut, int roomQuantity)
        //{
        //    if (string.IsNullOrEmpty(idsJson) || string.IsNullOrEmpty(checkIn) || string.IsNullOrEmpty(checkOut))
        //    {
        //        TempData["Error"] = "Thiếu dữ liệu: idsJson, checkIn hoặc checkOut đang null.";
        //        return RedirectToAction("Index");
        //    }


        //    var ids = JsonConvert.DeserializeObject<List<int>>(idsJson);
        //    var parsedCheckIn = DateTime.ParseExact(checkIn, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        //    var parsedCheckOut = DateTime.ParseExact(checkOut, "yyyy-MM-dd", CultureInfo.InvariantCulture);

        //    var roomTypes = await _roomTypeService.GetByIds(ids);

        //    var viewModel = new RoomsViewModel
        //    {
        //        RoomTypes = roomTypes,
        //        BookingInfo = new CreateBookingDto
        //        {
        //            CheckInDate = parsedCheckIn,
        //            CheckOutDate = parsedCheckOut,
        //            RoomQuantity = roomQuantity
        //        }
        //    };

        //    return View(viewModel);
        //}
        public async Task<IActionResult> Rooms(string? idsJson, string? checkIn, string? checkOut, int? roomQuantity)
        {
            List<RoomType> roomTypes;

            if (!string.IsNullOrEmpty(idsJson) && !string.IsNullOrEmpty(checkIn) && !string.IsNullOrEmpty(checkOut) && roomQuantity.HasValue)
            {
                // Gọi từ form "Check availability"
                var ids = JsonConvert.DeserializeObject<List<int>>(idsJson);
                roomTypes = await _roomTypeService.GetByIds(ids);

                var parsedCheckIn = DateTime.ParseExact(checkIn, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var parsedCheckOut = DateTime.ParseExact(checkOut, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                var viewModel = new RoomsViewModel
                {
                    RoomTypes = roomTypes,
                    BookingInfo = new CreateBookingDto
                    {
                        CheckInDate = parsedCheckIn,
                        CheckOutDate = parsedCheckOut,
                        RoomQuantity = roomQuantity.Value
                    }
                };

                return View(viewModel);
            }

            // Truy cập từ navbar hoặc đường dẫn trực tiếp → hiển thị tất cả
            roomTypes = await _roomTypeService.GetAll();
            var defaultViewModel = new RoomsViewModel
            {
                RoomTypes = roomTypes,
                BookingInfo = new CreateBookingDto() // rỗng để tránh null
            };

            return View(defaultViewModel);
        }



        public IActionResult Restaurant()
        {
            return View();
        }
        public IActionResult Spa()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult News()
        {
            return View();
        }
        public IActionResult Service()
        {
            return View();
        }
        //public IActionResult RoomDetails()
        //{
        //    return View();
        //}
        public async Task<IActionResult> RoomDetails(int id)
        {
            var roomType = await _roomTypeService.GetById(id);

            return View(roomType);
        }

        [HttpPost]
        public async Task<IActionResult> CheckAvailability([FromBody] CheckingAvailableRoomDto dto)
        {
            var roomTypeIds = await _bookingService.GetAvailableRoomTypeIdsAsync(
                dto.CheckIn,
                dto.CheckOut,
                dto.Adults,
                dto.Children,
                dto.RoomQuantity
            );

            if (!roomTypeIds.Any())
            {
                return Json(new
                {
                    success = false,
                    message = "Không còn phòng trống phù hợp với yêu cầu của bạn !"
                });
            }

            var redirectUrl = Url.Action("Rooms", new
            {
                idsJson = JsonConvert.SerializeObject(roomTypeIds),
                checkIn = dto.CheckIn.ToString("yyyy-MM-dd"),
                checkOut = dto.CheckOut.ToString("yyyy-MM-dd"),
                roomQuantity = dto.RoomQuantity
            });

            return Json(new
            {
                success = true,
                redirectUrl
            });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBooking(CreateBookingDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", new { error = "Bạn cần đăng nhập để đặt phòng." });
            }

            dto.UserId = user.Id; // 👈 Gán ID người dùng đang đăng nhập

            var result = await _bookingService.CreateBooking(dto);
            if (result)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index", new { error = "Đặt phòng không thành công. Vui lòng thử lại." });
        }
        [HttpPost]
        public async Task<IActionResult> ValidateAvailability([FromBody] CreateBookingDto dto)
        {
            Console.WriteLine("Check-in: " + dto.CheckInDate.ToString("yyyy-MM-dd HH:mm:ss"));
            Console.WriteLine("Check-out: " + dto.CheckOutDate.ToString("yyyy-MM-dd HH:mm:ss"));

            if (dto.CheckOutDate.Date <= dto.CheckInDate.Date)
            {
                return Json(new { success = false, message = "Ngày trả phòng phải sau ngày nhận phòng." });
            }

            var isAvailable = await _bookingService.CheckAvailability(dto);
            if (isAvailable)
            {
                await _bookingService.CreateBooking(dto);
                return Json(new { success = true });

            }    


            return Json(new { success = false, message = "Không còn phòng trống." });
        }
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> ConfirmBookingWithCheck([FromBody] CreateBookingDto dto)
        {
            if (dto.CheckOutDate <= dto.CheckInDate)
            {
                return Json(new { success = false, message = "Ngày trả phòng phải sau ngày nhận phòng." });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để đặt phòng." });
            }

            dto.UserId = user.Id;

            var isAvailable = await _bookingService.CheckAvailability(dto);
            if (!isAvailable)
            {
                return Json(new { success = false, message = "Không còn phòng trống cho ngày bạn chọn." });
            }

            var created = await _bookingService.CreateBooking(dto);
            if (!created)
            {
                return Json(new { success = false, message = "Đặt phòng thất bại. Vui lòng thử lại." });
            }

            return Json(new { success = true });
        }
        [Authorize]
        public async Task<IActionResult> MyBookings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bookings = await _bookingService.GetUserBookingsAsync(userId);
            return View(bookings);
        }
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            var dto = new UserDto
            {
                Email = user.Email,
                FullName = user.FullName,
                IdentityCard = user.IdentityCard,
                DOB = user.DOB,
                Address = user.Address,
                Gender = user.Gender
            };
            return View(dto);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UserDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            user.FullName = dto.FullName;
            user.IdentityCard = dto.IdentityCard;
            user.Address = dto.Address;
            user.DOB = dto.DOB;
            user.Gender = dto.Gender;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                TempData["Error"] = "Failed to update.";
                return View("Profile", dto);
            }
            TempData["Success"] = "Updated successfully.";
            return RedirectToAction("Profile");
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
            {
                TempData["Error"] = "Failed to change password.";
                return View("ChangePassword");
            }
            TempData["Success"] = "Password changed successfully.";
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Auth");
        }




    }
}
