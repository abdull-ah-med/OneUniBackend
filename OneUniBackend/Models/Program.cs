using System;
using System.Collections.Generic;

namespace OneUniBackend.Models;

public partial class Program
{
    public Guid ProgramId { get; set; }

    public Guid? DepartmentId { get; set; }

    public string Name { get; set; } = null!;

    public string? DegreeType { get; set; }

    public int? DurationYears { get; set; }

    public int? TotalCreditHours { get; set; }

    public string? Description { get; set; }

    public string? EligibilityCriteria { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual Department? Department { get; set; }

    public virtual ICollection<MeritFormula> MeritFormulas { get; set; } = new List<MeritFormula>();
}
