using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User_API.DataObjects;
using User_API.Interfaces;
using User_API.Models;
using User_API.Services;

namespace User_API.Controllers;

[Authorize]
[ApiController]
[Route("/api/users")]
public class UserController(IRepository<User> repository) : ControllerBase, IUserController
{
    [Authorize(Policy = "AdminPolicy")]
    [HttpPost("/admin-create")]
    public IActionResult CreateUser(CreateUserDto newUser, bool isAdmin)
    {
        var validationResult = ValidateUser();

        if (validationResult.Result != null)
            return validationResult.Result;

        if (!Validation.ValidateDto(newUser, out var errorMessage))
            return BadRequest(errorMessage);
        
        var userName = User.FindFirst(ClaimTypes.Name)!.Value;

        if (repository.Find(n => n.UserName == newUser.UserName).Count != 0)
            return BadRequest("User with this username already exists");

        var user = new User(
            newUser.UserName,
            BCrypt.Net.BCrypt.HashPassword(newUser.Password),
            newUser.Name,
            newUser.Gender,
            newUser.Birthday,
            isAdmin,
            DateTime.Now,
            userName
        );

        repository.Create(user);

        return Ok("User created successfully!!");
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpGet("/admin-get")]
    public ActionResult<ResponseUserDto> GetUser(string userName)
    {
        var validationResult = ValidateUser();
    
        if (validationResult.Result != null)
            return validationResult.Result;
    
        var user = FindByUsername(userName);
    
        if (user == null)
            return NotFound("User with this username does not exists!");
    
        return Ok(DtoConverter<User, ResponseUserDto>.ToDto(user));
    }

    [HttpGet("/get")]
    public ActionResult<ResponseUserDto> GetUser()
    {
        var validationResult = ValidateUser();

        if (validationResult.Result != null)
            return validationResult.Result;

        var user = validationResult.Value;

        return Ok(DtoConverter<User, ResponseUserDto>.ToDto(user!));
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpGet("/admin-getallactive")]
    public ActionResult<ICollection<ResponseUserDto>> GetAllActiveUsers()
    {
        var validationResult = ValidateUser();

        if (validationResult.Result != null)
            return validationResult.Result;

        var users = repository.Find(n => n.RevokedOn == null);

        var userDtos = new List<ResponseUserDto>();

        foreach (var user in users)
            userDtos.Add(DtoConverter<User, ResponseUserDto>.ToDto(user));

        return Ok(userDtos);
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpGet("/admin-getallaboveage")]
    public ActionResult<ICollection<ResponseUserDto>> GetAllUsersAboveAge(int age)
    {
        var validationResult = ValidateUser();

        if (validationResult.Result != null)
            return validationResult.Result;

        var users = repository.Find(user => user.Birthday != null && CalculateAge(user.Birthday.Value) > age);

        var userDtos = new List<ResponseUserDto>();

        foreach (var user in users)
            userDtos.Add(DtoConverter<User, ResponseUserDto>.ToDto(user));

        return Ok(userDtos);
    }

    [HttpPut("/update")]
    public IActionResult UpdateUser(UpdateUserDto newUser)
    {
        var validationResult = ValidateUser();

        if (validationResult.Result != null)
            return validationResult.Result;
        
        if (!Validation.ValidateDto(newUser, out var errorMessage))
            return BadRequest(errorMessage);
        
        if (repository.Find(n => n.UserName == newUser.UserName).Count != 0)
            return BadRequest("User with this username already exists");
        
        var user = validationResult.Value;

        DtoConverter<User, UpdateUserDto>.UpdateWithDto(user!, newUser);

        user!.ModifiedBy = User.FindFirst(ClaimTypes.Name)!.Value;
        user.ModifiedOn = DateTime.Now;

        repository.Update(user);

        return Ok("You successfully updated your profile!");
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpPut("/admin-update")]
    public IActionResult UpdateUser(UpdateUserDto newUser, string userName, bool? isAdmin = null)
    {
        var validationResult = ValidateUser();

        if (validationResult.Result != null)
            return validationResult.Result;
        
        if (!Validation.ValidateDto(newUser, out var errorMessage))
            return BadRequest(errorMessage);
        
        if (repository.Find(n => n.UserName == newUser.UserName).Count != 0)
            return BadRequest("User with this username already exists");

        var user = FindByUsername(userName);

        if (user == null)
            return NotFound("User with this username does not exists!");

        DtoConverter<User, UpdateUserDto>.UpdateWithDto(user, newUser);

        if (isAdmin != null)
            user.Admin = isAdmin == true;

        user.ModifiedBy = User.FindFirst(ClaimTypes.Name)!.Value;
        user.ModifiedOn = DateTime.Now;

        repository.Update(user);

        return Ok(userName + "'s profile was updated successfully!");
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpPut("/admin-restore")]
    public IActionResult RestoreUser(string userName)
    {
        var validationResult = ValidateUser();

        if (validationResult.Result != null)
            return validationResult.Result;

        var user = FindByUsername(userName);

        if (user == null)
            return NotFound("User with this username does not exists!");

        user.RevokedBy = null;
        user.RevokedOn = null;

        user.ModifiedBy = User.FindFirst(ClaimTypes.Name)!.Value;
        user.ModifiedOn = DateTime.Now;

        repository.Update(user);

        return Ok(userName + " was restored successfully!");
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpPut("/admin-deactivate")]
    public IActionResult DeactivateUser(string userName)
    {
        var validationResult = ValidateUser();

        if (validationResult.Result != null)
            return validationResult.Result;

        var user = FindByUsername(userName);

        if (user == null)
            return NotFound("User with this username does not exists!");

        if (user.UserName == "admin")
            return BadRequest("Default admin can not be deactivated!");

        user.RevokedBy = User.FindFirst(ClaimTypes.Name)!.Value;
        user.RevokedOn = DateTime.Now;

        user.ModifiedBy = User.FindFirst(ClaimTypes.Name)!.Value;
        user.ModifiedOn = DateTime.Now;

        repository.Update(user);

        return Ok(userName + " was deactivated successfully!");
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpDelete("/admin-delete")]
    public IActionResult DeleteUser(string userName)
    {
        var validationResult = ValidateUser();

        if (validationResult.Result != null)
            return validationResult.Result;

        var user = FindByUsername(userName);

        if (user == null)
            return NotFound("User with this username does not exists!");
        
        if (user.UserName == "admin")
            return BadRequest("Default admin can not be deleted!");

        repository.Remove(user);

        return Ok(userName + " was deleted successfully!");
    }


    private ActionResult<List<string>> ValidateToken()
    {
        var tokenId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (tokenId == null)
            return BadRequest("Token id is unstated!");

        var tokenRole = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (tokenRole == null)
            return BadRequest("Token role in unstated!");

        var tokenUserName = User.FindFirst(ClaimTypes.Name)?.Value;

        if (tokenUserName == null)
            return BadRequest("Token username is unstated!");

        return new List<string> { tokenId, tokenRole, tokenUserName };
    }

    private ActionResult<User> ValidateUser()
    {
        var tokenResult = ValidateToken();

        if (tokenResult.Result != null)
            return tokenResult.Result;

        var id = tokenResult.Value![0];

        var user = repository.GetById(Guid.Parse(id));

        if (user == null || user.RevokedOn != null)
            return Unauthorized("You profile does not exists!");

        return user;
    }

    private User? FindByUsername(string userName)
    {
        try
        {
            var user = repository.Find(n => n.UserName == userName).First();
            return user;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    private int CalculateAge(DateTime birthday)
    {
        var today = DateTime.Today;
        var age = today.Year - birthday.Year;

        if (birthday.Date > today.AddYears(-age))
            age--;

        return age;
    }
}