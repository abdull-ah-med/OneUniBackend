using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using OneUniBackend.Enums;

namespace OneUniBackend.Entities;

public partial class StudentProfile
{
    public Guid ProfileId { get; set; }

    public Guid? UserId { get; set; }

    [Column("gender", TypeName = "gender_type")]
    public GenderType? Gender { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    [Column("id_document_type", TypeName = "id_document_type")]
    public IdDocumentType? IdDocumentType { get; set; }

    public string? Cnic { get; set; }

    public string? PassportNumber { get; set; }

    public string? NicopNumber { get; set; }

    public string? GuardianName { get; set; }

    public string? FatherName { get; set; }

    [Column("guardian_relation", TypeName = "guardian_relation")]
    public GuardianRelation? GuardianRelation { get; set; }

    public string? GuardianPhone { get; set; }

    public string? GuardianCnic { get; set; }

    public string? GuardianCity { get; set; }

    public string? GuardianAddress { get; set; }

    public string? City { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? ProfilePictureUrl { get; set; }

    public bool? ScholarshipPriority { get; set; }

    public bool? HostelPriority { get; set; }

    public decimal? GuardianIncome { get; set; }

    public string? PreferredAdmissionCity { get; set; }

    public decimal? HouseholdIncome { get; set; }

    public bool? IsHafizQuran { get; set; }

    public bool? IsOrphan { get; set; }

    [Column("disability", TypeName = "jsonb")]
    public JsonDocument? Disability { get; set; }

    [Column("sports", TypeName = "jsonb")]
    public JsonDocument? Sports { get; set; }

    public bool? ProfileCompleted { get; set; }

    public int? CompletionPercentage { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual User? User { get; set; }
}
