using System;
using System.Collections.Generic;

namespace OneUniBackend.Models;

public partial class TestScore
{
    public Guid ScoreId { get; set; }

    public Guid? UserId { get; set; }

    public string? TestName { get; set; }

    public int? TotalMarks { get; set; }

    public int? ObtainedMarks { get; set; }

    public decimal? Percentage { get; set; }

    public DateOnly? TestDate { get; set; }

    public int Year { get; set; }

    public string? RollNumber { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? User { get; set; }
    public TestType TestType { get; set; }
}
