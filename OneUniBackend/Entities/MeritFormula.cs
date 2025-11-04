using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using ProgramEntity = OneUniBackend.Entities.Program;

namespace OneUniBackend.Entities;

[Table("merit_formulas")]
public partial class MeritFormula
{
    [Key]
    [Column("formula_id")]
    public Guid FormulaId { get; set; }

    [Column("university_id")]
    public Guid? UniversityId { get; set; }

    [Column("program_id")]
    public Guid? ProgramId { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Column("test_weightage")]
    [Precision(5, 2)]
    public decimal? TestWeightage { get; set; }

    [Column("intermediate_weightage")]
    [Precision(5, 2)]
    public decimal? IntermediateWeightage { get; set; }

    [Column("matric_weightage")]
    [Precision(5, 2)]
    public decimal? MatricWeightage { get; set; }

    [Column("interview_weightage")]
    [Precision(5, 2)]
    public decimal? InterviewWeightage { get; set; }

    [Column("minimum_test_score")]
    [Precision(5, 2)]
    public decimal? MinimumTestScore { get; set; }

    [Column("minimum_intermediate_percentage")]
    [Precision(5, 2)]
    public decimal? MinimumIntermediatePercentage { get; set; }

    [Column("minimum_matric_percentage")]
    [Precision(5, 2)]
    public decimal? MinimumMatricPercentage { get; set; }

    [Column("academic_year")]
    [StringLength(10)]
    public string? AcademicYear { get; set; }

    [Column("is_active")]
    public bool? IsActive { get; set; }

    [Column("created_at", TypeName = "timestamp with time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp with time zone")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("ProgramId")]
    [InverseProperty("MeritFormulas")]
    public virtual ProgramEntity? Program { get; set; }

    [ForeignKey("UniversityId")]
    [InverseProperty("MeritFormulas")]
    public virtual University? University { get; set; }
}
