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
        public async Task<ActionResult<User>> Index()
        {
            List<User> users = await context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> Index(int id)
        {
            User user = await context.Users.FindAsync(id);
            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(UserRegisterVM uservm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            UserDTO userdto = await auth.Register(uservm);

            if (userdto == null)
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(Index), new { Id = userdto.Id }, userdto);
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenDTO>> Login(UserLoginVM uservm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            User user = await auth.Login(uservm);
            if (user == null)
            {
                return BadRequest();
            }

            //TokenDTO tokens = new TokenDTO();
            //tokens.AccesToken = client.RefreshToken;
            //tokens.RefreshToken = await jwttoken.SaveGenerateRefreshToken(client);

            // New method to fill tokens object attributes :
            TokenDTO tokens = new TokenDTO
            {
                AccesToken = jwttoken.GenerateToken(user),
                RefreshToken = await jwttoken.SaveGenerateRefreshToken(user)
            };
            
            return Ok(tokens);
        }

        // The scenario here is that whene the user made a login the JwtServcie create for him a accces and refresh 
        // token, the access token expire every 15 min for ex, now the user want to access in to secure endpoint 
        // the UseAuthentication middleware check if the access token is valid, if the dateExpire is end, he return 
        // 401 status code, and the front must call the refresh-token action, and the check if the refreshtoken 
        // is still valid, if yes he regenerate new access token
        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenDTO>> RefreshToken(RefreshTokenRequestDTO request) 
        {
            TokenDTO res = await jwttoken.RefreshTokens(request);
            if (res == null || res.AccesToken == null || request.RefreshToken == null)
            {
                return Unauthorized();
            }

            return res;
        }

        [Authorize] // Here we call the middleware UseAuthorize to check if the user has the access to this action
        [HttpGet("auth")]
        public IActionResult AuthEndpoint()
        {
            return Ok("Your authed");
        }

        [Authorize(Roles = "Client")] // Here only the user with Client role who are accessible to this endpoint
        // The token contains the Role claim that are filled by the role of user, so like that the middleware can 
        // check the role
        [HttpGet("authclient")]
        public IActionResult AuthClientEndpoint()
        {
            return Ok("Your authed as Client!");
        }
    }
}
