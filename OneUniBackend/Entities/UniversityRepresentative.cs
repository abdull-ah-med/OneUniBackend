using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

using OneUniBackend.Enums;
namespace OneUniBackend.Entities;

[Table("university_representatives")]
public partial class UniversityRepresentative
{
    [Key]
    [Column("rep_id")]
    public Guid RepId { get; set; }

    [Column("user_id")]
    public Guid? UserId { get; set; }

    [Column("university_id")]
    public Guid? UniversityId { get; set; }

    [Column("full_name")]
    [StringLength(255)]
    public string FullName { get; set; } = null!;

    [Column("designation")]
    [StringLength(255)]
    public string? Designation { get; set; }

    [Column("department")]
    [StringLength(255)]
    public string? Department { get; set; }

    [Column("phone")]
    [StringLength(20)]
    public string? Phone { get; set; }

    [Column("office_address")]
    public string? OfficeAddress { get; set; }

    [Column("is_official")]
    public bool? IsOfficial { get; set; }

    [Column("verification_document_url")]
    [StringLength(500)]
    public string? VerificationDocumentUrl { get; set; }

    [Column("permissions", TypeName = "jsonb")]
    public string? Permissions { get; set; }

    [Column("is_active")]
    public bool? IsActive { get; set; }

    [Column("verification_status", TypeName = "verification_status")]
    public VerificationStatus? VerificationStatus { get; set; }

    [Column("created_at", TypeName = "timestamp with time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp with time zone")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("UniversityId")]
    [InverseProperty("UniversityRepresentatives")]
    public virtual University? University { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UniversityRepresentatives")]
    public virtual User? User { get; set; }
}
