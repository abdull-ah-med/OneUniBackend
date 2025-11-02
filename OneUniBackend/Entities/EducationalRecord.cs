using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

using OneUni.Enums;
namespace OneUni.Entities;

[Table("educational_records")]
[Index("UserId", Name = "idx_educational_records_user_id")]
public partial class EducationalRecord
{
    [Key]
    [Column("record_id")]
    public Guid RecordId { get; set; }

    [Column("user_id")]
    public Guid? UserId { get; set; }

    [Column("education_type")]
    public EducationType EducationType { get; set; }

    [Column("institution_name")]
    [StringLength(255)]
    public string? InstitutionName { get; set; }

    [Column("board_university")]
    [StringLength(255)]
    public string? BoardUniversity { get; set; }

    [Column("roll_number")]
    [StringLength(50)]
    public string? RollNumber { get; set; }

    [Column("total_marks")]
    public int? TotalMarks { get; set; }

    [Column("obtained_marks")]
    public int? ObtainedMarks { get; set; }

    [Column("percentage")]
    [Precision(5, 2)]
    public decimal? Percentage { get; set; }

    [Column("grade")]
    [StringLength(10)]
    public string? Grade { get; set; }

    [Column("year_of_completion")]
    public int? YearOfCompletion { get; set; }

    [Column("is_result_awaited")]
    public bool? IsResultAwaited { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("EducationalRecords")]
    public virtual User? User { get; set; }
}
