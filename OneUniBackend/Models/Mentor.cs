using System;
using System.Collections.Generic;

namespace OneUniBackend.Models;

public partial class Mentor
{
    public Guid MentorId { get; set; }

    public Guid? UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Designation { get; set; }

    public string? CurrentInstitution { get; set; }

    public int? GraduationYear { get; set; }

    public string? FieldOfStudy { get; set; }

    public int? ExperienceYears { get; set; }

    public string? Phone { get; set; }

    public string? LinkedinUrl { get; set; }

    public string? Bio { get; set; }

    public string? ProfilePictureUrl { get; set; }

    public List<string>? Specializations { get; set; }

    public decimal? HourlyRate { get; set; }

    public string? AvailabilityHours { get; set; }

    public string? UniversityEmail { get; set; }

    public Guid? UniversityId { get; set; }

    public int? TotalSessions { get; set; }

    public decimal? AverageRating { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<MentorshipSession> MentorshipSessions { get; set; } = new List<MentorshipSession>();

    public virtual University? University { get; set; }

    public virtual User? User { get; set; }
}
