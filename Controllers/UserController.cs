using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using User_service.Data;
using User_service.DTOS;
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
        private IAuthService auth;
        private JwtService jwttoken;
        public UserController(AppDbContext context, IAuthService auth, JwtService token)
        {
            this.context = context;
            this.auth = auth;
            jwttoken = token;
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
        public async Task<ActionResult<ClientDTO>> Register(ClientRegisterVM clientvm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            ClientDTO clientdto = await auth.Register(clientvm);

            if (clientdto == null)
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(Index), new { Id = clientdto.Id }, clientdto);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(ClientLoginVM clientvm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            Client client = await auth.Login(clientvm);
            if (client == null)
            {
                return BadRequest();
            }

            string token = jwttoken.GenerateToken(client);
            return Ok(token);
        }

        [Authorize] // Here we call the middleware UseAuthorize to check if the user has the access to this action
        [HttpGet("auth")]
        public IActionResult AuthEndpoint()
        {
            return Ok("Your authed");
        }
    }
}
