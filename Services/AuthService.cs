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

        public async Task<Client?> Login(ClientLoginVM clientvm)
        {
            Client client = ClientMP.TransferDataFromClientVMToClient(clientvm);

            Client clientDB = await context.Clients.FirstOrDefaultAsync(c => c.Email == client.Email);
            if (clientDB == null)
            {
                return null;
            }

            var dehashe = new PasswordHasher<Client>();
            var checkPass = dehashe.VerifyHashedPassword(clientDB, clientDB.MotPasse, client.MotPasse);
            if (checkPass == PasswordVerificationResult.Failed)
            {
                return null;
            }
            return clientDB;
        }
    }
}
