using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using OneUni.Enums;

namespace OneUni.Entities;

[Table("users")]
[Index("Email", Name = "idx_users_email")]
[Index("Email", Name = "users_email_key", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("email")]
    [StringLength(255)]
    public string Email { get; set; } = null!;
    [Column("full_name")]
    [StringLength(255)]
    public string? FullName { get; set; }
    [Column("password_hash")]
    [StringLength(255)]
    public string PasswordHash { get; set; } = null!;

    [Column("role")]
    public UserRole Role { get; set; }

    [Column("is_active")]
    public bool? IsActive { get; set; }

    [Column("is_verified")]
    public bool? IsVerified { get; set; }

    [Column("verification_token")]
    [StringLength(255)]
    public string? VerificationToken { get; set; }

    [Column("password_reset_token")]
    [StringLength(255)]
    public string? PasswordResetToken { get; set; }

    [Column("password_reset_expires", TypeName = "timestamp without time zone")]
    public DateTime? PasswordResetExpires { get; set; }

    [Column("last_login", TypeName = "timestamp without time zone")]
    public DateTime? LastLogin { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [Column("deleted_at", TypeName = "timestamp without time zone")]
    public DateTime? DeletedAt { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    [InverseProperty("User")]
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    [InverseProperty("User")]
    public virtual ICollection<Document> DocumentUsers { get; set; } = new List<Document>();

    [InverseProperty("VerifiedByNavigation")]
    public virtual ICollection<Document> DocumentVerifiedByNavigations { get; set; } = new List<Document>();

    [InverseProperty("User")]
    public virtual ICollection<EducationalRecord> EducationalRecords { get; set; } = new List<EducationalRecord>();

    [InverseProperty("User")]
    public virtual ICollection<Mentor> Mentors { get; set; } = new List<Mentor>();

    [InverseProperty("Student")]
    public virtual ICollection<MentorshipSession> MentorshipSessions { get; set; } = new List<MentorshipSession>();

    [InverseProperty("User")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [InverseProperty("User")]
    public virtual ICollection<StudentProfile> StudentProfiles { get; set; } = new List<StudentProfile>();

    [InverseProperty("User")]
    public virtual ICollection<TestScore> TestScores { get; set; } = new List<TestScore>();

    [InverseProperty("User")]
    public virtual ICollection<UniversityRepresentative> UniversityRepresentatives { get; set; } = new List<UniversityRepresentative>();

    [InverseProperty("User")]
    public virtual ICollection<UserLogin> UserLogins { get; set; } = new List<UserLogin>();

    [InverseProperty("User")]
    public virtual ICollection<UserRefreshToken> UserRefreshTokens { get; set; } = new List<UserRefreshToken>();
}
