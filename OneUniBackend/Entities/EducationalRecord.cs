using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using OneUniBackend.Enums;

namespace OneUniBackend.Entities;

public partial class EducationalRecord
{
    public Guid RecordId { get; set; }

    public Guid? UserId { get; set; }

    [Column("education_type", TypeName = "education_type")]
    public EducationType EducationType { get; set; }

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

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public virtual User? User { get; set; }
}
