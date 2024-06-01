using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using User_API.Interfaces;
using User_API.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using User_API.DataObjects;
using User_API.Services;

namespace User_API.Controllers;

[ApiController]
[Route("/api/auth")]
public class AuthenticationController(IRepository<User> repository, IConfiguration configuration) : ControllerBase, IAuthenticationController
{
    private string CreateToken(User user)
    {
        List<Claim> claims = [
            new Claim(ClaimTypes.NameIdentifier, user.Guid.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Admin? "Admin" : "User" )
        ];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:Token").Value!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    [HttpPost("/register")]
    public IActionResult Register(CreateUserDto newUser)
    {
        if (!Validation.ValidateDto(newUser, out var errorMessage))
            return BadRequest(errorMessage);
        
        if (repository.Find(n => n.UserName == newUser.UserName).Count != 0)
            return BadRequest("User with this username already exists");
        
        var user = new User(
            newUser.UserName, 
            BCrypt.Net.BCrypt.HashPassword(newUser.Password), 
            newUser.Name,
            newUser.Gender, 
            newUser.Birthday, 
            false, 
            DateTime.Now, 
            newUser.UserName
            );
        
        repository.Create(user);

        return Ok("Registration successful!");
    }

    [HttpPost("/login")]
    public IActionResult Login(string userName, string password)
    {
        var user = repository.Find(n => n.UserName == userName);
        
        if (user.Count == 0)
            return NotFound();

        if (!BCrypt.Net.BCrypt.Verify(password, user.First().HashPassword))
            return Unauthorized();

        return Ok(CreateToken(user.First()));
    }
    
}