using Microsoft.AspNetCore.Mvc;
using User_API.DataObjects;

namespace User_API.Interfaces;

public interface IAuthenticationController
{
    public IActionResult Register(CreateUserDto newUser);
    public IActionResult Login(string userName, string password);
    
    // public IActionResult UpdatePassword(string newPassword);
    //
    // public IActionResult UpdateUserName(string newUserName);

}