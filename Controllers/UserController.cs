using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using User_service.Data;
using User_service.DTOS;
using User_service.Mappers;
using User_service.Models;
using User_service.Services;
using User_service.ViewModels;

namespace User_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private AppDbContext context;
        private JwtService Jwttoken;
        private IAuthService auth;
        public UserController(AppDbContext context, JwtService token, IAuthService auth)
        {
            this.context = context;
            Jwttoken = token;
            this.auth = auth;
        }

        [HttpGet]
        public async Task<ActionResult<Client>> Index()
        {
            List<Client> clients = await context.Clients.ToListAsync();
            return Ok(clients);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> Index(int id)
        {
            Client client = await context.Clients.FindAsync(id);
            return Ok(client);
        }

        [HttpPost("register")]
        public ActionResult<ClientDTO> Register(ClientRegisterVM clientvm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            ClientDTO clientdto = auth.Register(clientvm);
            if (clientdto == null)
            {
                return BadRequest();
            }
            //Client client = ClientMP.TransferDataFromClientVMToClient(clientvm);

            //bool checkClient = context.Clients.Any(c => c.NomUser == client.NomUser || c.Email == client.Email);
            //if (checkClient)
            //{
            //    return BadRequest();
            //}

            //// Hashing the password of the object coming from the frontend
            //var passHashed = new PasswordHasher<Client>();
            //client.MotPasse = passHashed.HashPassword(client, client.MotPasse);
            //context.Add(client);
            //context.SaveChanges();
            //ClientDTO clientdto = ClientMP.ClientToClientDTO(client);
            return CreatedAtAction(nameof(Index), new { Id = clientdto.Id }, clientdto);
        }

        [HttpPost("login")]
        public ActionResult Login(ClientLoginVM clientvm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("1");
            }

            Client client = ClientMP.TransferDataFromClientVMToClient(clientvm);

            Client clientDB = context.Clients.FirstOrDefault(c => c.Email == client.Email);
            if (clientDB == null)
            {
                return BadRequest("2");
            }

            var dehashe = new PasswordHasher<Client>();
            var checkPass = dehashe.VerifyHashedPassword(clientDB, clientDB.MotPasse, client.MotPasse);
            if (checkPass == PasswordVerificationResult.Failed)
            {
                return BadRequest("3");
            }
            string token = Jwttoken.GenerateToken(clientDB);
            return Ok(token);
        }
    }
}
