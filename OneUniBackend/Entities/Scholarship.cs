using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OneUni.Entities;

[Table("scholarships")]
public partial class Scholarship
{
    [Key]
    [Column("scholarship_id")]
    public Guid ScholarshipId { get; set; }

    [Column("university_id")]
    public Guid? UniversityId { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("minimum_merit")]
    [Precision(8, 4)]
    public decimal? MinimumMerit { get; set; }

    [Column("income_criteria")]
    [Precision(12, 2)]
    public decimal? IncomeCriteria { get; set; }

    [Column("region_specific")]
    [StringLength(100)]
    public string? RegionSpecific { get; set; }

    [Column("coverage_percentage")]
    [Precision(5, 2)]
    public decimal? CoveragePercentage { get; set; }

    [Column("coverage_amount")]
    [Precision(12, 2)]
    public decimal? CoverageAmount { get; set; }

    [Column("additional_benefits")]
    public string? AdditionalBenefits { get; set; }

    [Column("application_deadline")]
    public DateOnly? ApplicationDeadline { get; set; }

    [Column("academic_year")]
    [StringLength(10)]
    public string? AcademicYear { get; set; }

    [Column("is_active")]
    public bool? IsActive { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("UniversityId")]
    [InverseProperty("Scholarships")]
    public virtual University? University { get; set; }
}
