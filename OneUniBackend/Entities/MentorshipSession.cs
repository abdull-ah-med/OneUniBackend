using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using OneUniBackend.Enums;

namespace OneUniBackend.Entities;

public partial class MentorshipSession
{
    public Guid SessionId { get; set; }

    public Guid? MentorId { get; set; }

    public Guid? StudentId { get; set; }

    [Column("session_type", TypeName = "session_type")]
    public SessionType SessionType { get; set; }

    public DateTime ScheduledAt { get; set; }

    public int? DurationMinutes { get; set; }

    [Column("session_status", TypeName = "session_status")]
    public SessionStatus SessionStatus { get; set; }

    public DateTime? ActualStartTime { get; set; }

    public DateTime? ActualEndTime { get; set; }

    public string? Topic { get; set; }

    public string? SessionNotes { get; set; }

    public string? MentorFeedback { get; set; }

    public string? StudentFeedback { get; set; }

    public decimal? FeeAmount { get; set; }

    public string? PaymentStatus { get; set; }

    public string? PaymentReference { get; set; }

    public int? MentorRating { get; set; }

    public int? StudentRating { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Mentor? Mentor { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual User? Student { get; set; }
}
