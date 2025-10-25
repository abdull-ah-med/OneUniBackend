using System;
using System.Collections.Generic;

namespace OneUniBackend.Models;

public partial class EducationalRecord
{
    public Guid RecordId { get; set; }

    public Guid? UserId { get; set; }

    public string? InstitutionName { get; set; }

    public string? BoardUniversity { get; set; }

    public string? RollNumber { get; set; }

    public int? TotalMarks { get; set; }

    public int? ObtainedMarks { get; set; }

    public decimal? Percentage { get; set; }

    public string? Grade { get; set; }

    public int? YearOfCompletion { get; set; }

    public bool? IsResultAwaited { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public EducationType EducationType { get; set; }
    public virtual User? User { get; set; }
}
