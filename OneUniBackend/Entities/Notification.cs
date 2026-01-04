using System;
using System.Collections.Generic;

namespace OneUniBackend.Entities;

public partial class Notification
{
    public Guid NotificationId { get; set; }

    public Guid? UserId { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string? Type { get; set; }

    public DateTime? ReadAt { get; set; }

    public Guid? RelatedApplicationId { get; set; }

    public Guid? RelatedSessionId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Application? RelatedApplication { get; set; }

    public virtual MentorshipSession? RelatedSession { get; set; }

    public virtual User? User { get; set; }
}
