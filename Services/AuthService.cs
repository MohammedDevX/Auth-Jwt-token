using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using User_service.Data;
using User_service.DTOS;
using User_service.Mappers;
using User_service.Models;
using User_service.ViewModels;

namespace User_service.Services
{
    public class AuthService(AppDbContext context, IConfiguration config): IAuthService
    {
        public async Task<ClientDTO?> Register(ClientRegisterVM clientvm)
        {
            Client client = ClientMP.TransferDataFromClientVMToClient(clientvm);

            bool checkClient = await context.Clients.AnyAsync(c => c.NomUser == client.NomUser || c.Email == client.Email);
            if (checkClient)
            {
                return null;
            }

            // Hashing the password of the object coming from the frontend
            var passHashed = new PasswordHasher<Client>();
            client.MotPasse = passHashed.HashPassword(client, client.MotPasse);
            await context.AddAsync(client);
            await context.SaveChangesAsync();
            ClientDTO clientdto = ClientMP.ClientToClientDTO(client);
            return clientdto;
        }

        public async Task<string> Login(ClientLoginVM clientvm)
        {

        }
    }
}
