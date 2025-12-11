using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using User_service.Data;
using User_service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

IConfiguration config = builder.Configuration;

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<JwtService>();

// Here we announce the server that we are using Jwt bearer token
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
    options =>
    {
        // Here we mention the rules that UseAuthenticaton middleware have to check if the token is valid or not
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration.GetValue<string>("AppSettings:Issuer"),
            ValidateAudience = true,
            ValidAudience = builder.Configuration.GetValue<string>("AppSettings:Audience"),
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("AppSettings:Token")!)),
            ValidateIssuerSigningKey = true
        };
        // N.B : So whene the user send a login request the middleware UseAuthentication, the client send the jwt 
        // token if his connected and the middleware check if this token is valid, if yes he create
        // HttpContext.User with claims, now when we send request to an endpoint who have Authorize annotation 
        // the middleware UseAuthorization see if the HttpContext.User is filled, if no he return 401 status code 
        // Not athentified, and if he is filled and he doesnt have the role he return 403 status code, not 
        // authorized to this action 
    });

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(
    option =>
    {
        option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString"));
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
