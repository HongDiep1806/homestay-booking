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

        public async Task<bool> Delete(int id)
        {
            return await _userRepository.DeleteUser(id);
        }

        public async Task<List<UserDto>> GetAllCustomers()
        {
            return _mapper.Map<List<UserDto>>(await _userRepository.GetAllCustomers());
        }

        public async Task<AppUser> GetById(int id)
        {
            var user = await _userRepository.GetById(id);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            return user;
        }

        public async Task<bool> Update(int id, AppUser user)
        {
            var existingUser = await _userRepository.GetById(id);
            if (existingUser == null)
            {
                return false;
            }
            existingUser.FullName = user.FullName;
            existingUser.IdentityCard = user.IdentityCard;
            existingUser.Gender = user.Gender;
            existingUser.DOB = user.DOB;
            existingUser.Address = user.Address;
            existingUser.IsActive = user.IsActive;

            try
            {
                await _userRepository.Update(id, existingUser);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in UserService.Update: " + ex.Message);
                return false;
            }
        }
    }
}
