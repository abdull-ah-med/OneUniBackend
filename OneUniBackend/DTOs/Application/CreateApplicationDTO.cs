using System.ComponentModel.DataAnnotations;

namespace OneUni.DTOs.Application;

public class CreateApplicationDTO
{
    [Required]
    public Guid UniversityId { get; set; }

    [Required]
    public Guid ProgramId { get; set; }

    [Required]
    public Guid CycleId { get; set; }

    public bool ScholarshipApplied { get; set; }
    public bool HostelRequired { get; set; }
    public bool TransportRequired { get; set; }
    public DateOnly? ScheduledSubmissionDate { get; set; }
}

