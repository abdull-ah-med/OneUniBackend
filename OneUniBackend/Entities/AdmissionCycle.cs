using System;
using System.Collections.Generic;

namespace OneUniBackend.Entities;

public partial class AdmissionCycle
{
    public Guid CycleId { get; set; }

    public Guid? UniversityId { get; set; }

    public string AcademicYear { get; set; } = null!;

    public string? SessionName { get; set; }

    public DateOnly? ApplicationStartDate { get; set; }

    public DateOnly? ApplicationEndDate { get; set; }

    public DateOnly? TestDate { get; set; }

    public DateOnly? MeritListDate { get; set; }

    public DateOnly? FeeSubmissionDeadline { get; set; }

    public DateOnly? ClassesStartDate { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual University? University { get; set; }
}
