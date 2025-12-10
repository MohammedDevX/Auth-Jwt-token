using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using User_service.DTOS;
using User_service.Models;
using User_service.ViewModels;

namespace User_service.Mappers
{
    public class ClientMP
    {
        public static Client TransferDataFromClientVMToClient(ClientRegisterVM clientvm)
        {
            return new Client
            {
                NomUser = clientvm.NomUser,
                Email = clientvm.Email,
                MotPasse = clientvm.MotPasse
            };
        }

        public static Client TransferDataFromClientVMToClient(ClientLoginVM clientvm)
        {
            return new Client
            {
                Email = clientvm.Email,
                MotPasse = clientvm.MotPasse
            };
        }

        public static ClientDTO ClientToClientDTO(Client client)
        {
            return new ClientDTO
            {
                Id = client.Id,
                NomUser = client.NomUser,
                Email = client.Email,
            };
        }
    }
}
