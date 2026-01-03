using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

using OneUniBackend.Enums;
namespace OneUniBackend.Entities;

[Table("documents")]
[Index("ApplicationId", Name = "idx_documents_application_id")]
[Index("UserId", Name = "idx_documents_user_id")]
[Index("EducationalRecordId", Name = "idx_documents_educational_record_id")]
public partial class Document
{
    [Key]
    [Column("document_id")]
    public Guid DocumentId { get; set; }

    [Column("user_id")]
    public Guid? UserId { get; set; }

    [Column("application_id")]
    public Guid? ApplicationId { get; set; }

    [Column("educational_record_id")]
    public Guid? EducationalRecordId { get; set; }

    [Column("document_name")]
    [StringLength(255)]
    public string DocumentName { get; set; } = null!;

    [Column("document_type", TypeName = "document_type")]
    public DocumentType DocumentType { get; set; }

    [Column("file_path")]
    [StringLength(500)]
    public string FilePath { get; set; } = null!;

    [Column("bucket")]
    [StringLength(200)]
    public string? Bucket { get; set; }

    [Column("object_key")]
    [StringLength(500)]
    public string? ObjectKey { get; set; }

    [Column("storage_provider")]
    [StringLength(50)]
    public string? StorageProvider { get; set; }

    [Column("checksum")]
    [StringLength(200)]
    public string? Checksum { get; set; }

    [Column("file_size")]
    public int? FileSize { get; set; }

    [Column("mime_type")]
    [StringLength(100)]
    public string? MimeType { get; set; }

    [Column("verified_by")]
    public Guid? VerifiedBy { get; set; }

    [Column("verified_at", TypeName = "timestamp with time zone")]
    public DateTime? VerifiedAt { get; set; }

    [Column("verification_status", TypeName = "verification_status")]
    public VerificationStatus? VerificationStatus { get; set; }

    [Column("rejection_reason")]
    public string? RejectionReason { get; set; }

    [Column("is_required")]
    public bool? IsRequired { get; set; }

    [Column("display_order")]
    public int? DisplayOrder { get; set; }

    [Column("metadata", TypeName = "jsonb")]
    public JsonDocument? Metadata { get; set; }

    [Column("created_at", TypeName = "timestamp with time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp with time zone")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("ApplicationId")]
    [InverseProperty("Documents")]
    public virtual Application? Application { get; set; }

    [ForeignKey("EducationalRecordId")]
    [InverseProperty("Documents")]
    public virtual EducationalRecord? EducationalRecord { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("DocumentUsers")]
    public virtual User? User { get; set; }

    [ForeignKey("VerifiedBy")]
    [InverseProperty("DocumentVerifiedByNavigations")]
    public virtual User? VerifiedByNavigation { get; set; }
}
