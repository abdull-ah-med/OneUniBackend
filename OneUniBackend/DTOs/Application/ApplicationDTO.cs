using OneUniBackend.Enums;

namespace OneUniBackend.DTOs.Application;

public class ApplicationDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid UniversityId { get; set; }
    public string UniversityName { get; set; } = null!;
    public Guid ProgramId { get; set; }
    public string ProgramName { get; set; } = null!;
    public Guid CycleId { get; set; }
    public string? ApplicationNumber { get; set; }
    public DateTime? SubmissionDate { get; set; }
    public ApplicationStatus Status { get; set; }
    public bool ScholarshipApplied { get; set; }
    public bool HostelRequired { get; set; }
    public bool TransportRequired { get; set; }
    public decimal? CalculatedMerit { get; set; }
    public int? MeritPosition { get; set; }
    public bool AdmissionOffered { get; set; }
    public DateOnly? OfferDate { get; set; }
    public DateOnly? OfferExpiresAt { get; set; }
    public DateTime? CreatedAt { get; set; }
}

