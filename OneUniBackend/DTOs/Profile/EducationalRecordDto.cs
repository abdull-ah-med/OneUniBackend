using System.ComponentModel.DataAnnotations;
using OneUniBackend.Enums;

namespace OneUniBackend.DTOs.Profile;

public class EducationalRecordDto
{
    public Guid? RecordId { get; set; }

    [Required]
    public EducationType EducationType { get; set; }

    [MaxLength(255)]
    public string? InstitutionName { get; set; }

    [MaxLength(255)]
    public string? BoardUniversity { get; set; }

    [MaxLength(50)]
    public string? RollNumber { get; set; }

    public int? TotalMarks { get; set; }
    public int? ObtainedMarks { get; set; }
    public decimal? Percentage { get; set; }

    [MaxLength(10)]
    public string? Grade { get; set; }

    public int? YearOfCompletion { get; set; }
    public bool? IsResultAwaited { get; set; }
}

