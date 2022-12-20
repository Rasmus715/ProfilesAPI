namespace AuthorizationAPI.Models;

public class Account : Raven.Identity.IdentityUser
{
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}