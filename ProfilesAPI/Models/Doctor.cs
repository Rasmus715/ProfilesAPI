namespace ProfilesAPI.Models;

public class Doctor : IEntity
{
    public string Id { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string MiddleName { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public Guid? AccountId { get; set; }
    public Guid? SpecializationId { get; set; }
    public Guid? OfficeId { get; set; }
    public int CareerStartYear { get; set; }
}