namespace AuthorizationAPI.Models;

public class Account
{
    public Guid Id { get; set; }
    public string PasswordHash { get; set; } = null!; 
    public string? PhoneNumber { get; set; }
    public string Email { get; set; } = null!;
    public bool IsEmailVerified { get; set; }
    public Guid? PhotoId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}