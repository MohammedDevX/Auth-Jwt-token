using User_service.DTOS;
using User_service.Models;
using User_service.ViewModels;

namespace User_service.Services
{
    public interface IAuthService
    {
        public Task<ClientDTO?> Register(ClientRegisterVM client);
        public Task<string> Client(ClientLoginVM client);
    }
}
