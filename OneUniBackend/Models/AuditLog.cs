using System;
using System.Collections.Generic;
using System.Net;

namespace OneUniBackend.Models;

public partial class AuditLog
{
    public Guid LogId { get; set; }

    public Guid? UserId { get; set; }

    public string Action { get; set; } = null!;

    public string? TableName { get; set; }

    public Guid? RecordId { get; set; }

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public IPAddress? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
