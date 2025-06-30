using System.Threading.Tasks;
using HomestayBooking.DTOs.BookingDto;
using HomestayBooking.DTOs.UserDto;
using HomestayBooking.Models;
using HomestayBooking.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HomestayBooking.Controllers
{
    [Authorize(Roles = "Admin, Staff")]
    public class AdminController : Controller
    {
        private readonly IRoomTypeService _roomTypeService;
        private readonly IRoomService _roomService;
        private readonly IUserService _userService;
        private readonly IBookingService _bookingService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        public AdminController(IRoomTypeService roomTypeService, IRoomService roomService, IUserService userService, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IBookingService bookingService)
        {
            _roomTypeService = roomTypeService;
            _roomService = roomService;
            _userService = userService;
            _bookingService = bookingService;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var bookings = await _bookingService.GetBookingAsync(); 
            var recentBookings = bookings
                .OrderByDescending(b => b.BookingID)
                .Where(b => b.Status != BookingStatus.Cancelled)
                .Take(5)
                .ToList();

            var rooms = await _roomService.GetAll(); 
            var availableRooms = rooms.Count(r => r.RoomStatus == true); 


            var roomTypeRatios = bookings
                .Where(b => b != null && b.RoomType != null) 
                .GroupBy(b => b.RoomType.Name)
                .Select(g => new RoomTypeRatioItem
                {
                    RoomTypeName = g.Key,
                    Count = g.Count()
                })
                .ToList();

            var model = new HotelDashboardViewModel
            {
                TotalBookings = bookings.Count(),
                AvailableRooms = availableRooms,
                RoomTypeRatios = roomTypeRatios,
                RecentBookings = recentBookings
            };

            return View(model);
        }


        public async Task<IActionResult> AllBooking()
        {
            var bookings = await _bookingService.GetBookingAsync();
            return View(bookings);
        }
        [HttpGet]
        public async Task<IActionResult> AddBooking()
        {
            try
            {
                var customers = await _userService.GetAllCustomers();
                var roomTypes = await _roomTypeService.GetAll();

                ViewBag.Customers = new SelectList(customers, "Id", "FullName");
                ViewBag.RoomTypes = new SelectList(roomTypes, "RoomTypeID", "Name");

                var dto = new CreateBookingByStaffDto
                {
                    CheckInDate = DateTime.Today,
                    CheckOutDate = DateTime.Today.AddDays(1)
                };

                return View(dto);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải dữ liệu đặt phòng: " + ex.Message;
                return RedirectToAction("AllBooking");
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddBooking(CreateBookingByStaffDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.CustomerId))
                errors.Add("Vui lòng chọn khách hàng.");
            if (dto.RoomTypeID <= 0)
                errors.Add("Vui lòng chọn loại phòng.");
            if (dto.CheckInDate == default || dto.CheckOutDate == default)
                errors.Add("Vui lòng nhập ngày nhận và trả phòng.");
            if (dto.CheckInDate >= dto.CheckOutDate)
                errors.Add("Ngày trả phòng phải sau ngày nhận phòng.");
            if (dto.RoomQuantity <= 0)
                errors.Add("Số lượng phòng phải lớn hơn 0.");

            if (errors.Any())
            {
                var customers = await _userService.GetAllCustomers();
                var roomTypes = await _roomTypeService.GetAll();

                ViewBag.Customers = new SelectList(customers, "Id", "FullName");
                ViewBag.RoomTypes = new SelectList(roomTypes, "RoomTypeID", "Name");
                ViewBag.Errors = errors;

                return View(dto);
            }

            try
            {
                await _bookingService.CreateBookingByStaffAsync(dto);
                TempData["Success"] = "Thêm booking thành công!";
                return RedirectToAction("AllBooking");
            }
            catch (Exception ex)
            {
                var customers = await _userService.GetAllCustomers();
                var roomTypes = await _roomTypeService.GetAll();

                ViewBag.Customers = new SelectList(customers, "Id", "FullName");
                ViewBag.RoomTypes = new SelectList(roomTypes, "RoomTypeID", "Name");

                TempData["Error"] = "Đã xảy ra lỗi khi thêm booking: " + ex.Message;
                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditBooking(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null) return NotFound();

            var customers = await _userService.GetAllCustomers();
            var roomTypes = await _roomTypeService.GetAll();
            var statuses = Enum.GetValues(typeof(BookingStatus))
                .Cast<BookingStatus>()
                .Select(s => new SelectListItem { Text = s.ToString(), Value = s.ToString() });

            ViewBag.Customers = new SelectList(customers, "Id", "FullName", booking.CustomerId);
            ViewBag.RoomTypes = new SelectList(roomTypes, "RoomTypeID", "Name", booking.RoomTypeID);
            ViewBag.Statuses = new SelectList(statuses, "Value", "Text", booking.Status);

            return View(booking);
        }
        [HttpPost]
        public async Task<IActionResult> EditBooking(int id, Booking booking)
        {
            var existingBooking = await _bookingService.GetBookingByIdAsync(id);
            if (existingBooking == null) return NotFound();

            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(booking.CustomerId))
                errors.Add("Vui lòng chọn khách hàng.");
            if (booking.RoomTypeID <= 0)
                errors.Add("Vui lòng chọn loại phòng.");
            if (booking.RoomQuantity <= 0)
                errors.Add("Số lượng phòng không hợp lệ.");
            if (booking.CheckIn >= booking.CheckOut)
                errors.Add("Ngày trả phòng phải sau ngày nhận phòng.");

            if (errors.Any())
            {
                ViewBag.Errors = errors;

                var customers = await _userService.GetAllCustomers();
                var roomTypes = await _roomTypeService.GetAll();
                var statuses = Enum.GetValues(typeof(BookingStatus))
                    .Cast<BookingStatus>()
                    .Select(s => new SelectListItem { Text = s.ToString(), Value = s.ToString() });

                ViewBag.Customers = new SelectList(customers, "Id", "FullName", booking.CustomerId);
                ViewBag.RoomTypes = new SelectList(roomTypes, "RoomTypeID", "Name", booking.RoomTypeID);
                ViewBag.Statuses = new SelectList(statuses, "Value", "Text", booking.Status.ToString());

                return View(booking);
            }

            existingBooking.CustomerId = booking.CustomerId;
            existingBooking.RoomTypeID = booking.RoomTypeID;
            existingBooking.RoomQuantity = booking.RoomQuantity;
            existingBooking.CheckIn = booking.CheckIn;
            existingBooking.CheckOut = booking.CheckOut;
            existingBooking.Status = booking.Status;

            await _bookingService.UpdateBookingAsync(id, existingBooking);
            TempData["Success"] = "Cập nhật booking thành công!";
            return RedirectToAction("AllBooking");
        }



        public async Task<IActionResult> AllCustomer()
        {
            var customers = await _userService.GetAllCustomers();
            return View(customers);
        }

        [HttpGet]
        public IActionResult AddCustomer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(UserDto dto)
        {
            dto.Password = "Customer123";
            var user = new AppUser
            {
                FullName = dto.FullName,
                IdentityCard = dto.IdentityCard,
                Gender = dto.Gender,
                DOB = dto.DOB,
                Address = dto.Address,
                Email = dto.Email,
                UserName = dto.Email,
                IsActive = dto.IsActive,
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return RedirectToAction("AddCustomer", "Admin");

            await _userManager.AddToRoleAsync(user, "Customer");
            return RedirectToAction("AllCustomer", "Admin");
        }

        [HttpGet]
        public async Task<IActionResult> EditCustomer(string email)
        {
            var user = await _userService.GetByEmail(email);
            if (user == null)
            {
                return NotFound();
            }

            var dto = new UserDto
            {
                Email = user.Email,
                FullName = user.FullName,
                IdentityCard = user.IdentityCard,
                Gender = user.Gender,
                DOB = user.DOB,
                Address = user.Address,
                IsActive = user.IsActive
            };

            return View(dto);
        }


        [HttpPost]
        public async Task<IActionResult> EditCustomer(UserDto dto)
        {

            var existingUser = await _userService.GetByEmail(dto.Email);
            if (existingUser == null)
            {
                TempData["Error"] = "User not found.";
                return View(dto);
            }

            existingUser.FullName = dto.FullName;
            existingUser.IdentityCard = dto.IdentityCard;
            existingUser.Gender = dto.Gender;
            existingUser.DOB = dto.DOB;
            existingUser.Address = dto.Address;
            existingUser.IsActive = dto.IsActive;

            var result = await _userService.UpdateByEmail(dto.Email, existingUser);

            if (!result)
            {
                TempData["Error"] = "Failed to update customer.";
                return View(dto);
            }

            TempData["Success"] = "Customer updated successfully.";
            return RedirectToAction("AllCustomer");
        }



        [HttpGet]
        public async Task<IActionResult> AllRoom()
        {
            var rooms = await _roomService.GetAll();
            return View(rooms);
        }

        [HttpGet]
        public async Task<IActionResult> AddRoom()
        {
            var roomTypes = await _roomTypeService.GetAll();
            ViewBag.RoomTypes = roomTypes;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRoom(Room room, IFormFile RoomImageFile)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(room.RoomCode))
                errors.Add("Room code is required.");

            if (room.RoomTypeID <= 0)
                errors.Add("Room type must be selected.");

            if (RoomImageFile == null || RoomImageFile.Length == 0)
                errors.Add("An image file must be uploaded.");

            if (errors.Any())
            {
                ViewBag.Errors = errors;
                ViewBag.RoomTypes = await _roomTypeService.GetAll();
                return View(room);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(RoomImageFile.FileName);
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await RoomImageFile.CopyToAsync(stream);
            }

            room.RoomImg = "/img/" + fileName;
            room.RoomStatus = true;

            await _roomService.Create(room);

            return RedirectToAction("AllRoom");
        }


        [HttpGet]
        public async Task<IActionResult> EditRoom(int id)
        {
            var room = await _roomService.GetById(id);
            if (room == null)
                return NotFound();
            var roomTypes = await _roomTypeService.GetAll();
            ViewBag.RoomTypes = new SelectList(roomTypes, "RoomTypeID", "Name", room.RoomTypeID);
            return View(room);
        }

        [HttpPost]
        public async Task<IActionResult> EditRoom(Room room, IFormFile? RoomImageFile)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(room.RoomCode))
                errors.Add("Room code is required.");
            if (room.RoomTypeID <= 0)
                errors.Add("Room type must be selected.");
            if (errors.Any())
            {
                ViewBag.Errors = errors;
                ViewBag.RoomTypes = await _roomTypeService.GetAll();
                return View(room);
            }
            if (RoomImageFile != null && RoomImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(RoomImageFile.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);
                var filePath = Path.Combine(folderPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await RoomImageFile.CopyToAsync(stream);
                }
                room.RoomImg = "/img/" + fileName;
            }
            await _roomService.Update(room.RoomID, room);
            return RedirectToAction("AllRoom");
        }

        public async Task<IActionResult> AllStaff()
        {
            var staffs = await _userService.GetAllStaffs();
            return View(staffs);
        }
        [HttpGet]
        public IActionResult AddStaff()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddStaff(UserDto dto)
        {
            dto.Password = $"{dto.FullName.Split(' ')[^1]}HotelStaff";
            var user = new AppUser
            {
                FullName = dto.FullName,
                IdentityCard = dto.IdentityCard,
                Gender = dto.Gender,
                DOB = dto.DOB,
                Address = dto.Address,
                Email = dto.Email,
                UserName = dto.Email,
                IsActive = dto.IsActive,
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return RedirectToAction("AddStaff", "Admin");

            await _userManager.AddToRoleAsync(user, "Staff");
            return RedirectToAction("AllStaff", "Admin");
        }

        [HttpGet]
        public async Task<IActionResult> EditStaff(string email)
        {
            var user = await _userService.GetByEmail(email);
            if (user == null)
            {
                return NotFound();
            }

            var dto = new UserDto
            {
                Email = user.Email,
                FullName = user.FullName,
                IdentityCard = user.IdentityCard,
                Gender = user.Gender,
                DOB = user.DOB,
                Address = user.Address,
                IsActive = user.IsActive
            };

            return View(dto);
        }


        [HttpPost]
        public async Task<IActionResult> EditStaff(UserDto dto)
        {

            var existingUser = await _userService.GetByEmail(dto.Email);
            if (existingUser == null)
            {
                TempData["Error"] = "User not found.";
                return View(dto);
            }

            existingUser.FullName = dto.FullName;
            existingUser.IdentityCard = dto.IdentityCard;
            existingUser.Gender = dto.Gender;
            existingUser.DOB = dto.DOB;
            existingUser.Address = dto.Address;
            existingUser.IsActive = dto.IsActive;

            var result = await _userService.UpdateByEmail(dto.Email, existingUser);

            if (!result)
            {
                TempData["Error"] = "Failed to update customer.";
                return View(dto);
            }

            TempData["Success"] = "Customer updated successfully.";
            return RedirectToAction("AllStaff");
        }

        public async Task<IActionResult> Invoice()
        {
            var invoices = await _bookingService.GetInvoiceAsync();
            return View(invoices);
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
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

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                TempData["Error"] = "Failed to update profile.";
                return View("Profile", dto);
            }
            TempData["Success"] = "Profile updated successfully.";
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

        public IActionResult Error404()
        {
            return View();
        }

        public IActionResult Error500()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var result = await _roomService.Delete(id);
            if (!result)
            {
                TempData["Error"] = "Failed to delete room.";
            }
            return RedirectToAction("AllRoom");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCustomer(string id)
        {
            var result = await _userService.Delete(id);
            if (!result)
            {
                TempData["Error"] = "Failed to delete room.";
            }
            return RedirectToAction("AllCustomer");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStaff(string id)
        {
            var result = await _userService.Delete(id);
            if (!result)
            {
                TempData["Error"] = "Failed to delete room.";
            }
            return RedirectToAction("AllStaff");
        }
    }
}
