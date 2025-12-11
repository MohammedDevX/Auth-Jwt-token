using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using User_service.Data;
using User_service.DTOS;
using User_service.Mappers;
using User_service.Models;
using User_service.ViewModels;

namespace User_service.Services
{
    public class AuthService(AppDbContext context): IAuthService
    {
        public async Task<UserDTO?> Register(UserRegisterVM uservm)
        {
            User user = UserMP.TransferDataFromUserVMToUser(uservm);

            bool checkuser = await context.Users.AnyAsync(c => c.NomUser == user.NomUser || c.Email == user.Email);
            if (checkuser)
            {
                return null;
            }

            // Hashing the password of the object coming from the frontend
            var passHashed = new PasswordHasher<User>();
            user.MotPasse = passHashed.HashPassword(user, user.MotPasse);
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            
            if (user.Role.ToString() == "Client")
            {
                Client client = UserMP.UserToClient(user);
                await context.Clients.AddAsync(client);
            } else
            {
                Admin admin = UserMP.UserToAdmin(user);
                await context.Admins.AddAsync(admin);
            }

            await context.SaveChangesAsync();

            UserDTO userdto = UserMP.UserToUserDTO(user);
            return userdto;
        }

        public async Task<User?> Login(UserLoginVM uservm)
        {
            User user = UserMP.TransferDataFromUserVMToUser(uservm);

            User userDB = await context.Users.FirstOrDefaultAsync(c => c.Email == user.Email);
            if (userDB == null)
            {
                return null;
            }

            var dehashe = new PasswordHasher<User>();
            var checkPass = dehashe.VerifyHashedPassword(userDB, userDB.MotPasse, user.MotPasse);
            if (checkPass == PasswordVerificationResult.Failed)
            {
                return null;
            }
            return userDB;
        }
    }
}
