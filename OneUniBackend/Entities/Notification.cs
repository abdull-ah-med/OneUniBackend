using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OneUni.Entities;

[Table("notifications")]
[Index("UserId", Name = "idx_notifications_user_id")]
public partial class Notification
{
    [Key]
    [Column("notification_id")]
    public Guid NotificationId { get; set; }

    [Column("user_id")]
    public Guid? UserId { get; set; }

    [Column("title")]
    [StringLength(255)]
    public string Title { get; set; } = null!;

    [Column("message")]
    public string Message { get; set; } = null!;

    [Column("type")]
    [StringLength(50)]
    public string? Type { get; set; }

    [Column("read_at", TypeName = "timestamp without time zone")]
    public DateTime? ReadAt { get; set; }

    [Column("related_application_id")]
    public Guid? RelatedApplicationId { get; set; }

    [Column("related_session_id")]
    public Guid? RelatedSessionId { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("RelatedApplicationId")]
    [InverseProperty("Notifications")]
    public virtual Application? RelatedApplication { get; set; }

    [ForeignKey("RelatedSessionId")]
    [InverseProperty("Notifications")]
    public virtual MentorshipSession? RelatedSession { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Notifications")]
    public virtual User? User { get; set; }
}
