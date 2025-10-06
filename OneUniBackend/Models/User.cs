using System;
using System.Collections.Generic;

namespace OneUniBackend.Models;

public partial class User
{
    public Guid UserId { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public UserRole Role { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsVerified { get; set; }

    public string? VerificationToken { get; set; }

    public string? PasswordResetToken { get; set; }

    public DateTime? PasswordResetExpires { get; set; }

    public DateTime? LastLogin { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<Document> DocumentUsers { get; set; } = new List<Document>();

    public virtual ICollection<Document> DocumentVerifiedByNavigations { get; set; } = new List<Document>();

    public virtual ICollection<EducationalRecord> EducationalRecords { get; set; } = new List<EducationalRecord>();

    public virtual ICollection<Mentor> Mentors { get; set; } = new List<Mentor>();

    public virtual ICollection<MentorshipSession> MentorshipSessions { get; set; } = new List<MentorshipSession>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<StudentProfile> StudentProfiles { get; set; } = new List<StudentProfile>();

    public virtual ICollection<TestScore> TestScores { get; set; } = new List<TestScore>();

    public virtual ICollection<UniversityRepresentative> UniversityRepresentatives { get; set; } = new List<UniversityRepresentative>();
}
