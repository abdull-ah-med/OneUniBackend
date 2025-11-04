using OneUniBackend.Enums;

namespace OneUniBackend.DTOs.Mentor;

public class MentorDTO
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public string? Designation { get; set; }
    public string? CurrentInstitution { get; set; }
    public string? FieldOfStudy { get; set; }
    public int? ExperienceYears { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public List<string>? Specializations { get; set; }
    public decimal? HourlyRate { get; set; }
    public decimal? AverageRating { get; set; }
    public int TotalSessions { get; set; }
    public VerificationStatus VerificationStatus { get; set; }
    public bool IsActive { get; set; }
}

