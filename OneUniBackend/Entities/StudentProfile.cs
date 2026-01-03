using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

using OneUniBackend.Enums;
namespace OneUniBackend.Entities;

[Table("student_profiles")]
[Index("UserId", Name = "idx_student_profiles_user_id")]
public partial class StudentProfile
{
    [Key]
    [Column("profile_id")]
    public Guid ProfileId { get; set; }

    [Column("user_id")]
    public Guid? UserId { get; set; }

    [Column("full_name")]
    [StringLength(255)]
    public string FullName { get; set; } = null!;

    [Column("date_of_birth")]
    public DateOnly? DateOfBirth { get; set; }

    [Column("gender", TypeName = "gender_type")]
    public GenderType? Gender { get; set; }

    [Column("id_document_type", TypeName = "id_document_type")]
    public IdDocumentType? IdDocumentType { get; set; }

    [Column("cnic")]
    [StringLength(15)]
    public string? Cnic { get; set; }

    [Column("passport_number")]
    [StringLength(20)]
    public string? PassportNumber { get; set; }

    [Column("nicop_number")]
    [StringLength(20)]
    public string? NicopNumber { get; set; }

    [Column("guardian_name")]
    [StringLength(255)]
    public string? GuardianName { get; set; }

    [Column("father_name")]
    [StringLength(255)]
    public string? FatherName { get; set; }

    [Column("guardian_relation", TypeName = "guardian_relation")]
    public GuardianRelation? GuardianRelation { get; set; }

    [Column("guardian_phone")]
    [StringLength(20)]
    public string? GuardianPhone { get; set; }

    [Column("guardian_cnic")]
    [StringLength(15)]
    public string? GuardianCnic { get; set; }

    [Column("guardian_city")]
    [StringLength(100)]
    public string? GuardianCity { get; set; }

    [Column("guardian_address")]
    public string? GuardianAddress { get; set; }

    [Column("city")]
    [StringLength(100)]
    public string? City { get; set; }

    [Column("address")]
    public string? Address { get; set; }

    [Column("phone")]
    [StringLength(20)]
    public string? Phone { get; set; }

    [Column("profile_picture_url")]
    [StringLength(500)]
    public string? ProfilePictureUrl { get; set; }

    [Column("scholarship_priority")]
    public bool? ScholarshipPriority { get; set; }

    [Column("hostel_priority")]
    public bool? HostelPriority { get; set; }

    [Column("guardian_income")]
    [Precision(12, 2)]
    public decimal? GuardianIncome { get; set; }

    [Column("preferred_admission_city")]
    [StringLength(100)]
    public string? PreferredAdmissionCity { get; set; }

    [Column("household_income")]
    [Precision(12, 2)]
    public decimal? HouseholdIncome { get; set; }

    [Column("is_hafiz_quran")]
    public bool? IsHafizQuran { get; set; }

    [Column("is_orphan")]
    public bool? IsOrphan { get; set; }

    [Column("disability", TypeName = "jsonb")]
    public JsonDocument? Disability { get; set; }

    [Column("sports", TypeName = "jsonb")]
    public JsonDocument? Sports { get; set; }

    [Column("profile_completed")]
    public bool? ProfileCompleted { get; set; }

    [Column("completion_percentage")]
    public int? CompletionPercentage { get; set; }

    [Column("created_at", TypeName = "timestamp with time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp with time zone")]
    public DateTime? UpdatedAt { get; set; }

    [Column("deleted_at", TypeName = "timestamp with time zone")]
    public DateTime? DeletedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("StudentProfiles")]
    public virtual User? User { get; set; }
}
