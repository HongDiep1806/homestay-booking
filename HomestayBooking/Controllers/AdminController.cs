using System.Threading.Tasks;
using HomestayBooking.DTOs.UserDto;
using HomestayBooking.Models;
using HomestayBooking.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HomestayBooking.Controllers
{
    public class AdminController : Controller
    {
        private readonly IRoomTypeService _roomTypeService;
        private readonly IRoomService _roomService;
        private readonly IUserService _userService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        public AdminController(IRoomTypeService roomTypeService, IRoomService roomService, IUserService userService, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _roomTypeService = roomTypeService;
            _roomService = roomService;
            _userService = userService;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AllBooking()
        {
            return View();
        }

        public IActionResult AddBooking()
        {
            return View();
        }

        public IActionResult EditBooking()
        {
            return View();
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

        public IActionResult Invoice()
        {
            return View();
        }

        public IActionResult CreateInvoice()
        {
            return View();
        }

        public IActionResult InvoiceReport()
        {
            return View();
        }

        public IActionResult AllBlog()
        {
            return View();
        }

        public IActionResult AddBlog()
        {
            return View();
        }

        public IActionResult EditBlog()
        {
            return View();
        }

        public IActionResult BlogDetail()
        {
            return View();
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult EditProfile()
        {
            return View();
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
