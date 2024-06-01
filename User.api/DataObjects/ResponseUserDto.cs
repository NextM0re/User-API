namespace User_API.DataObjects;

public class ResponseUserDto(
    string name,
    int gender,
    DateTime? birthday,
    DateTime? revokedOn)
{
    public string Name { get; set; } = name;

    public int Gender { get; set; } = gender;
    
    public DateTime? Birthday { get; set; } = birthday;

    public DateTime? RevokedOn { get; set; } = revokedOn;

}