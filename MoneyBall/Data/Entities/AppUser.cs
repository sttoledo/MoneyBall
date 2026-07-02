namespace MoneyBall.Data.Entities;

public enum UserRole
{
    Admin,
    Restricted
}

public class AppUser
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string DisplayName { get; set; }
    public required string PasswordHash { get; set; }
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
}
