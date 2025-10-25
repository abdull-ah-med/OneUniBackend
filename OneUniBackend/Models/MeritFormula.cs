using System;
using System.Collections.Generic;

namespace OneUniBackend.Models;

public partial class MeritFormula
{
    public Guid FormulaId { get; set; }

    public Guid? UniversityId { get; set; }

    public Guid? ProgramId { get; set; }

    public string Name { get; set; } = null!;

    public decimal? TestWeightage { get; set; }

    public decimal? IntermediateWeightage { get; set; }

    public decimal? MatricWeightage { get; set; }

    public decimal? InterviewWeightage { get; set; }

    public decimal? MinimumTestScore { get; set; }

    public decimal? MinimumIntermediatePercentage { get; set; }

    public decimal? MinimumMatricPercentage { get; set; }

    public string? AcademicYear { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Programs? Programs { get; set; }

    public virtual University? University { get; set; }
}
