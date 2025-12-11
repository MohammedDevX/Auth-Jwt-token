using User_service.DTOS;
using User_service.Models;
using User_service.ViewModels;

namespace User_service.Services
{
    public interface IAuthService
    {
        public Task<UserDTO?> Register(UserRegisterVM user);
        public Task<User?> Login(UserLoginVM user);
    }
}
