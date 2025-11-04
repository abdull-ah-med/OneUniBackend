using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

using OneUniBackend.Enums;
namespace OneUniBackend.Entities;

[Table("test_scores")]
[Index("UserId", Name = "idx_test_scores_user_id")]
public partial class TestScore
{
    [Key]
    [Column("score_id")]
    public Guid ScoreId { get; set; }

    [Column("user_id")]
    public Guid? UserId { get; set; }

    [Column("test_type", TypeName = "test_type")]
    public TestType TestType { get; set; }

    [Column("test_name")]
    [StringLength(255)]
    public string? TestName { get; set; }

    [Column("total_marks")]
    public int? TotalMarks { get; set; }

    [Column("obtained_marks")]
    public int? ObtainedMarks { get; set; }

    [Column("percentage")]
    [Precision(5, 2)]
    public decimal? Percentage { get; set; }

    [Column("test_date")]
    public DateOnly? TestDate { get; set; }

    [Column("year")]
    public int Year { get; set; }

    [Column("roll_number")]
    [StringLength(50)]
    public string? RollNumber { get; set; }

    [Column("created_at", TypeName = "timestamp with time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp with time zone")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("TestScores")]
    public virtual User? User { get; set; }
}
