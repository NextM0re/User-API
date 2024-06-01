using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User_API.DataObjects;
using User_API.Models;

namespace User_API.Interfaces;

public interface IUserController
{
    // Create
    public IActionResult CreateUser(CreateUserDto newUser, bool isAdmin);
    
    // Read
    public ActionResult<ResponseUserDto> GetUser(string userName);

    public ActionResult<ResponseUserDto> GetUser();

    public ActionResult<ICollection<ResponseUserDto>> GetAllActiveUsers();

    public ActionResult<ICollection<ResponseUserDto>> GetAllUsersAboveAge(int age);
    
    // Update
    public IActionResult UpdateUser(UpdateUserDto newUser);

    public IActionResult UpdateUser(UpdateUserDto newUser, string userName, bool? isAdmin = null);
    
    public IActionResult RestoreUser(string userName);
    
    public IActionResult DeactivateUser(string userName);
    
    // Delete
    public IActionResult DeleteUser(string userName);
}
