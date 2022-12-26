namespace ProfilesAPI.Models;

public class Patient : IEntity
{
    public string Id { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public bool IsLinkedToAccount { get; set; }
    public DateTime DateOfBirth {get;set;}
    public Guid? AccountId { get; set; }
}