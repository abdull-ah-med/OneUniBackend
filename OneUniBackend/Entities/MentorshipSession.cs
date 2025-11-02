using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

using OneUni.Enums;
namespace OneUni.Entities;

[Table("mentorship_sessions")]
[Index("MentorId", Name = "idx_mentorship_sessions_mentor_id")]
[Index("StudentId", Name = "idx_mentorship_sessions_student_id")]
public partial class MentorshipSession
{
    [Key]
    [Column("session_id")]
    public Guid SessionId { get; set; }

    [Column("mentor_id")]
    public Guid? MentorId { get; set; }

    [Column("student_id")]
    public Guid? StudentId { get; set; }

    [Column("session_type")]
    public SessionType SessionType { get; set; }

    [Column("session_status")]
    public SessionStatus SessionStatus { get; set; }

    [Column("scheduled_at", TypeName = "timestamp without time zone")]
    public DateTime ScheduledAt { get; set; }

    [Column("duration_minutes")]
    public int? DurationMinutes { get; set; }

    [Column("actual_start_time", TypeName = "timestamp without time zone")]
    public DateTime? ActualStartTime { get; set; }

    [Column("actual_end_time", TypeName = "timestamp without time zone")]
    public DateTime? ActualEndTime { get; set; }

    [Column("topic")]
    [StringLength(255)]
    public string? Topic { get; set; }

    [Column("session_notes")]
    public string? SessionNotes { get; set; }

    [Column("mentor_feedback")]
    public string? MentorFeedback { get; set; }

    [Column("student_feedback")]
    public string? StudentFeedback { get; set; }

    [Column("fee_amount")]
    [Precision(8, 2)]
    public decimal? FeeAmount { get; set; }

    [Column("payment_status")]
    [StringLength(50)]
    public string? PaymentStatus { get; set; }

    [Column("payment_reference")]
    [StringLength(100)]
    public string? PaymentReference { get; set; }

    [Column("mentor_rating")]
    public int? MentorRating { get; set; }

    [Column("student_rating")]
    public int? StudentRating { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("MentorId")]
    [InverseProperty("MentorshipSessions")]
    public virtual Mentor? Mentor { get; set; }

    [InverseProperty("RelatedSession")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [ForeignKey("StudentId")]
    [InverseProperty("MentorshipSessions")]
    public virtual User? Student { get; set; }
}
