using System;
using System.Collections.Generic;

namespace OneUniBackend.Entities;

public partial class Scholarship
{
    public Guid ScholarshipId { get; set; }

    public Guid? UniversityId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? MinimumMerit { get; set; }

    public decimal? IncomeCriteria { get; set; }

    public string? RegionSpecific { get; set; }

    public decimal? CoveragePercentage { get; set; }

    public decimal? CoverageAmount { get; set; }

    public string? AdditionalBenefits { get; set; }

    public DateOnly? ApplicationDeadline { get; set; }

    public string? AcademicYear { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual University? University { get; set; }
}
