using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using User_service.Data;
using User_service.DTOS;
using User_service.Models;

namespace User_service.Services
{
    public class JwtService
    {
        // We are going to injecte IConfiguration interface who able us to read values from env files, like 
        // appsetting.json, by giving the key
        private IConfiguration config;
        private AppDbContext context;
        public JwtService(IConfiguration config, AppDbContext context)
        {
            this.config = config;
            this.context = context;
        }

        public string GenerateToken(Client client)
        {
            // Here we have a List of Claim, Claim is an object who contain informations about the user, we are and 
            // we are going sauvgarde the clamis in the JWT token in the payload 
            // Every object Claim contain one information, so we create a List of claims who contain all informations 
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, client.Id.ToString()),
                new Claim(ClaimTypes.Email, client.Email),
                new Claim(ClaimTypes.Name, client.NomUser),
                new Claim(ClaimTypes.Role, client.Role.ToString()),
            };

            // Cette class permet de generer un key secret qui nous permet de signer le jwt puisque que l'api le suivre
            // Donc depuis maintenant le serveur connait notre token avec ce key secret
            var key = new SymmetricSecurityKey(
                // Here de config attribut injected able us to get the secret key of toke in Appsettings file 
                // We have something like => "Token": "The string secret key"
                // After that we convert the string secret key in to bytes
                Encoding.UTF8.GetBytes(config.GetValue<string>("Appsettings:Token")!)
            );

            // Cette class permet de signer le jwt, on utilisant le key generer dans le code precedant, et 
            // on presisant l'algoritme de signature
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescription = new JwtSecurityToken(
                // The api or the name of application who created the token
                issuer: config.GetValue<string>("Appsettings:Issuer"), 
                // The client whos going to cosume the token
                audience: config.GetValue<string>("Appsettings:Audience"), 
                // claims : contain informations about the user
                claims: claims,
                // Date expirement
                expires: DateTime.UtcNow.AddDays(1),
                // Creds : la signature
                signingCredentials: creds
            );
            // Transforme the token to string => Header.Paylaod.Signature
            // 1) Header : type + algo (HmacSha512)
            // 2) Payload : issuer + audience + claims + expires
            // 3) Signature : signingCredentials
            return new JwtSecurityTokenHandler().WriteToken(tokenDescription);
        }

        private string GenerateRefreshToken()
        {
            var randNum = new byte[32]; // Byte table whith size 32 empty 
            using var rng = RandomNumberGenerator.Create(); // Create a generator numbres
            rng.GetBytes(randNum); // The generator fill every box in the table with 1 byterandomly
            return Convert.ToBase64String(randNum); // Convertir the table to a string
        }

        public async Task<string> SaveGenerateRefreshToken(Client client)
        {
            string refToken = GenerateRefreshToken();
            client.RefreshToken = refToken;
            client.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await context.SaveChangesAsync();
            return refToken;
        }

        private async Task<Client?> ValidateRefreshToken(int UserId, string RefreshToken)
        {
        Client client = await context.Clients.FindAsync(UserId);
            if (client == null || client.RefreshToken != RefreshToken || client.RefreshTokenExpiry <= DateTime.UtcNow)
            {
                return null;
            }

            return client;
        }

        public async Task<TokenDTO?> RefreshTokens(RefreshTokenRequestDTO request)
        {
            Client? client = await ValidateRefreshToken(request.UserId, request.RefreshToken);
            if (client == null)
            {
                return null;
            }

            return new TokenDTO
            {
                AccesToken = GenerateToken(client),
                RefreshToken = request.RefreshToken
            };
        }
    }
}
