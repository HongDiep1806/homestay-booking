using AutoMapper;
using HomestayBooking.DTOs.UserDto;
using HomestayBooking.Models;
using HomestayBooking.Repositories;

namespace HomestayBooking.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<bool> Delete(string id)
        {
            return await _userRepository.DeleteUser(id);
        }

        public async Task<List<UserDto>> GetAllCustomers()
        {
            return _mapper.Map<List<UserDto>>(await _userRepository.GetAllCustomers());
        }
        public async Task<List<UserDto>> GetAllStaffs()
        {
            return _mapper.Map<List<UserDto>>(await _userRepository.GetAllStaffs());
        }

        public async Task<AppUser> GetByEmail(string email)
        {
            var user = await _userRepository.GetByEmail(email);
            if (user == null)
                throw new Exception("User not found");
            return user;
        }

        public async Task<bool> UpdateByEmail(string email, AppUser user)
        {
            var existingUser = await _userRepository.GetByEmail(email);
            if (existingUser == null)
                return false;

            existingUser.FullName = user.FullName;
            existingUser.IdentityCard = user.IdentityCard;
            existingUser.Gender = user.Gender;
            existingUser.DOB = user.DOB;
            existingUser.Address = user.Address;
            existingUser.IsActive = user.IsActive;

            try
            {
                await _userRepository.Update(existingUser);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in UserService.UpdateByEmail: " + ex.Message);
                return false;
            }
        }
    }
}
