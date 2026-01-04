using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using OneUniBackend.Enums;

namespace OneUniBackend.Entities;

public partial class UniversityRepresentative
{
    public Guid RepId { get; set; }

    public Guid? UserId { get; set; }

    public Guid? UniversityId { get; set; }

    public string? Designation { get; set; }

    public string? Department { get; set; }

    public string? Phone { get; set; }

    public string? OfficeAddress { get; set; }

    public bool? IsOfficial { get; set; }

    [Column("verification_status", TypeName = "verification_status")]
    public VerificationStatus? VerificationStatus { get; set; }

    public string? VerificationDocumentUrl { get; set; }

    [Column("permissions", TypeName = "jsonb")]
    public string? Permissions { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual University? University { get; set; }

    public virtual User? User { get; set; }
}
