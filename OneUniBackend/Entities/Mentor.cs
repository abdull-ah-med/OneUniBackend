using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

using OneUni.Enums;
namespace OneUni.Entities;

[Table("mentors")]
public partial class Mentor
{
    [Key]
    [Column("mentor_id")]
    public Guid MentorId { get; set; }

    [Column("user_id")]
    public Guid? UserId { get; set; }

    [Column("full_name")]
    [StringLength(255)]
    public string FullName { get; set; } = null!;

    [Column("designation")]
    [StringLength(255)]
    public string? Designation { get; set; }

    [Column("current_institution")]
    [StringLength(255)]
    public string? CurrentInstitution { get; set; }

    [Column("graduation_year")]
    public int? GraduationYear { get; set; }

    [Column("field_of_study")]
    [StringLength(255)]
    public string? FieldOfStudy { get; set; }

    [Column("experience_years")]
    public int? ExperienceYears { get; set; }

    [Column("phone")]
    [StringLength(20)]
    public string? Phone { get; set; }

    [Column("linkedin_url")]
    [StringLength(500)]
    public string? LinkedinUrl { get; set; }

    [Column("bio")]
    public string? Bio { get; set; }

    [Column("profile_picture_url")]
    [StringLength(500)]
    public string? ProfilePictureUrl { get; set; }

    [Column("specializations")]
    public List<string>? Specializations { get; set; }

    [Column("hourly_rate")]
    [Precision(8, 2)]
    public decimal? HourlyRate { get; set; }

    [Column("availability_hours", TypeName = "jsonb")]
    public string? AvailabilityHours { get; set; }

    [Column("university_email")]
    [StringLength(255)]
    public string? UniversityEmail { get; set; }

    [Column("university_id")]
    public Guid? UniversityId { get; set; }

    [Column("total_sessions")]
    public int? TotalSessions { get; set; }

    [Column("average_rating")]
    [Precision(3, 2)]
    public decimal? AverageRating { get; set; }

    [Column("is_active")]
    public bool? IsActive { get; set; }

    [Column("verification_status")]
    public VerificationStatus? VerificationStatus { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("Mentor")]
    public virtual ICollection<MentorshipSession> MentorshipSessions { get; set; } = new List<MentorshipSession>();

    [ForeignKey("UniversityId")]
    [InverseProperty("Mentors")]
    public virtual University? University { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Mentors")]
    public virtual User? User { get; set; }
}
