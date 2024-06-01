using System.ComponentModel.DataAnnotations;

namespace User_API.DataObjects;

public class CreateUserDto
{   
    [StringLength(32, MinimumLength = 8)]
    public required string UserName { get; set; }
    
    [StringLength(16, MinimumLength = 8)]
    public required string Password { get; set; }
    
    [StringLength(32)]
    public required string Name { get; set; }
    public required int Gender { get; set; } 
    
    public DateTime? Birthday { get; set; }
}