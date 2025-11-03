using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OneUni.Entities;

[Table("programs")]
[Index("DepartmentId", Name = "idx_programs_department_id")]
public partial class Program
{
    [Key]
    [Column("program_id")]
    public Guid ProgramId { get; set; }

    [Column("department_id")]
    public Guid? DepartmentId { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Column("degree_type")]
    [StringLength(50)]
    public string? DegreeType { get; set; }

    [Column("duration_years")]
    public int? DurationYears { get; set; }

    [Column("total_credit_hours")]
    public int? TotalCreditHours { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("eligibility_criteria")]
    public string? EligibilityCriteria { get; set; }

    [Column("is_active")]
    public bool? IsActive { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime? UpdatedAt { get; set; }

    //[InverseProperty("Program")]
    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    [ForeignKey("DepartmentId")]
    [InverseProperty("Program")]
    public virtual Department? Department { get; set; }

    [InverseProperty("Program")]
    public virtual ICollection<MeritFormula> MeritFormulas { get; set; } = new List<MeritFormula>();
}
