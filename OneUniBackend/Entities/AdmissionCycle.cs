using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OneUni.Entities;

[Table("admission_cycles")]
public partial class AdmissionCycle
{
    [Key]
    [Column("cycle_id")]
    public Guid CycleId { get; set; }

    [Column("university_id")]
    public Guid? UniversityId { get; set; }

    [Column("academic_year")]
    [StringLength(10)]
    public string AcademicYear { get; set; } = null!;

    [Column("session_name")]
    [StringLength(50)]
    public string? SessionName { get; set; }

    [Column("application_start_date")]
    public DateOnly? ApplicationStartDate { get; set; }

    [Column("application_end_date")]
    public DateOnly? ApplicationEndDate { get; set; }

    [Column("test_date")]
    public DateOnly? TestDate { get; set; }

    [Column("merit_list_date")]
    public DateOnly? MeritListDate { get; set; }

    [Column("fee_submission_deadline")]
    public DateOnly? FeeSubmissionDeadline { get; set; }

    [Column("classes_start_date")]
    public DateOnly? ClassesStartDate { get; set; }

    [Column("is_active")]
    public bool? IsActive { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("Cycle")]
    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    [ForeignKey("UniversityId")]
    [InverseProperty("AdmissionCycles")]
    public virtual University? University { get; set; }
}
