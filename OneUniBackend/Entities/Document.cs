using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using OneUniBackend.Enums;

namespace OneUniBackend.Entities;

public partial class Document
{
    public Guid DocumentId { get; set; }

    public Guid? UserId { get; set; }

    public Guid? ApplicationId { get; set; }

    public Guid? EducationalRecordId { get; set; }

    [Column("document_type", TypeName = "document_type")]
    public DocumentType DocumentType { get; set; }

    public string DocumentName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public string? Bucket { get; set; }

    public string? ObjectKey { get; set; }

    public string? StorageProvider { get; set; }

    public string? Checksum { get; set; }

    public int? FileSize { get; set; }

    public string? MimeType { get; set; }

    [Column("metadata", TypeName = "jsonb")]
    public JsonDocument? Metadata { get; set; }

    [Column("verification_status", TypeName = "verification_status")]
    public VerificationStatus? VerificationStatus { get; set; }

    public Guid? VerifiedBy { get; set; }

    public DateTime? VerifiedAt { get; set; }

    public string? RejectionReason { get; set; }

    public bool? IsRequired { get; set; }

    public int? DisplayOrder { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Application? Application { get; set; }

    public virtual EducationalRecord? EducationalRecord { get; set; }

    public virtual User? User { get; set; }

    public virtual User? VerifiedByNavigation { get; set; }
}
