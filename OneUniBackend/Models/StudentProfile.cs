using System;
using System.Collections.Generic;

namespace OneUniBackend.Models;

public partial class StudentProfile
{
    public Guid ProfileId { get; set; }

    public Guid? UserId { get; set; }

    public string FullName { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    public string? Cnic { get; set; }

    public string? PassportNumber { get; set; }

    public string? NicopNumber { get; set; }

    public string? GuardianName { get; set; }

    public string? City { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? ProfilePictureUrl { get; set; }

    public bool? ScholarshipPriority { get; set; }

    public bool? HostelPriority { get; set; }

    public string? PreferredAdmissionCity { get; set; }

    public decimal? HouseholdIncome { get; set; }

    public bool? IsHafizQuran { get; set; }

    public bool? IsDisabled { get; set; }

    public string? Sports { get; set; }

    public bool? ProfileCompleted { get; set; }

    public int? CompletionPercentage { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual User? User { get; set; }
    public GenderType? Gender { get; set; }

    public IdDocumentType? IdDocumentType { get; set; }

    public GuardianRelation? GuardianRelation { get; set; }
}
