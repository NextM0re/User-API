using System.ComponentModel.DataAnnotations;

namespace User_API.DataObjects;

public class UpdateUserDto
{
    [StringLength(32, MinimumLength = 8)]
    public string? UserName { get; set; }
    
    [StringLength(16, MinimumLength = 8)]
    public string? Password { get; set; }
    
    [StringLength(32)]
    public string? Name { get; set; }

    public int? Gender { get; set; } 
    
    public DateTime? Birthday { get; set; }

}