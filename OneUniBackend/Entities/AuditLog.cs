using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace OneUni.Entities;

[Table("audit_logs")]
public partial class AuditLog
{
    [Key]
    [Column("log_id")]
    public Guid LogId { get; set; }

    [Column("user_id")]
    public Guid? UserId { get; set; }

    [Column("action")]
    [StringLength(100)]
    public string Action { get; set; } = null!;

    [Column("table_name")]
    [StringLength(100)]
    public string? TableName { get; set; }

    [Column("record_id")]
    public Guid? RecordId { get; set; }

    [Column("old_values", TypeName = "jsonb")]
    public string? OldValues { get; set; }

    [Column("new_values", TypeName = "jsonb")]
    public string? NewValues { get; set; }

    [Column("ip_address")]
    public IPAddress? IpAddress { get; set; }

    [Column("user_agent")]
    public string? UserAgent { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("AuditLogs")]
    public virtual User? User { get; set; }
}
