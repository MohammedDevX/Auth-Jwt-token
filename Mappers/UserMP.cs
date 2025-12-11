using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using User_service.DTOS;
using User_service.Models;
using User_service.ViewModels;

namespace User_service.Mappers
{
    public class UserMP
    {
        public static User TransferDataFromUserVMToUser(UserRegisterVM uservm)
        {
            return new User
            {
                NomUser = uservm.NomUser,
                Email = uservm.Email,
                MotPasse = uservm.MotPasse,
                Role = uservm.Role
            };
        }

        public static User TransferDataFromUserVMToUser(UserLoginVM uservm)
        {
            return new User
            {
                Email = uservm.Email,
                MotPasse = uservm.MotPasse
            };
        }

        public static UserDTO UserToUserDTO(User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                NomUser = user.NomUser,
                Email = user.Email,
            };
        }

        public static Client UserToClient(User user)
        {
            return new Client
            {
                UserId = user.Id
            };
        }

        public static Admin UserToAdmin(User user)
        {
            return new Admin
            {
                UserId = user.Id
            };
        }
    }
}
