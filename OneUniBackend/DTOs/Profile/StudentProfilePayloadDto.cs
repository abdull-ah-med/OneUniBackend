using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using OneUniBackend.Enums;

namespace OneUniBackend.DTOs.Profile;

public class StudentProfilePayloadDto
{
    public Guid? ProfileId { get; set; }

    [Required]
    [MaxLength(255)]
    public string FullName { get; set; } = string.Empty;

    public DateOnly? DateOfBirth { get; set; }
    public GenderType? Gender { get; set; }
    public IdDocumentType? IdDocumentType { get; set; }

    [MaxLength(15)]
    public string? Cnic { get; set; }

    [MaxLength(20)]
    public string? PassportNumber { get; set; }

    [MaxLength(20)]
    public string? NicopNumber { get; set; }

    [MaxLength(255)]
    public string? GuardianName { get; set; }

    [MaxLength(255)]
    public string? FatherName { get; set; }

    public GuardianRelation? GuardianRelation { get; set; }

    [MaxLength(20)]
    public string? GuardianPhone { get; set; }

    [MaxLength(15)]
    public string? GuardianCnic { get; set; }

    [MaxLength(100)]
    public string? GuardianCity { get; set; }

    public string? GuardianAddress { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    public string? Address { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(500)]
    public string? ProfilePictureUrl { get; set; }

    public bool? ScholarshipPriority { get; set; }
    public bool? HostelPriority { get; set; }
    public decimal? GuardianIncome { get; set; }
    public decimal? HouseholdIncome { get; set; }

    [MaxLength(100)]
    public string? PreferredAdmissionCity { get; set; }

    public bool? IsHafizQuran { get; set; }
    public bool? IsOrphan { get; set; }

    public JsonElement? Disability { get; set; }
    public JsonElement? Sports { get; set; }
}

