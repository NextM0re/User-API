using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace User_API.Models;

public class User(
    string userName,
    string hashPassword,
    string name,
    int gender,
    DateTime? birthday,
    bool admin,
    DateTime createdOn,
    string createdBy
)
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Guid { get; set; }
    
    [StringLength(32)]
    public  string UserName { get; set; } = userName;

    [StringLength(64)] 
    public string HashPassword { get; set; } = hashPassword;

    [StringLength(16)] 
    public string Name { get; set; } = name;

    public int Gender { get; set; } = gender;
    public DateTime? Birthday { get; set; } = birthday;

    public bool Admin { get; set; } = admin;

    public DateTime CreatedOn { get; set; } = createdOn;
    
    [StringLength(32)] 
    public string CreatedBy { get; set; } = createdBy;

    public DateTime? ModifiedOn { get; set; } = null;
    
    [StringLength(32)] 
    public string? ModifiedBy { get; set; } = null;
    
    public DateTime? RevokedOn { get; set; } = null;
    
    [StringLength(32)] 
    public string? RevokedBy { get; set; } = null;

}