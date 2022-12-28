using ProfilesAPI.Enums;

namespace ProfilesAPI.ViewModels;

public class DoctorViewModel
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string MiddleName { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public Guid? AccountId { get; set; } = null!;
    public Guid? SpecializationId { get; set; }
    public Guid? OfficeId { get; set; }
    public int CareerStartYear { get; set; }
    public string? Status { get; set; }
}