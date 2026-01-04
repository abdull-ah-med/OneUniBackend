using System;
using System.Collections.Generic;

namespace OneUniBackend.Entities;

public partial class University
{
    public Guid UniversityId { get; set; }

    public string Name { get; set; } = null!;

    public string? ShortName { get; set; }

    public string? Type { get; set; }

    public string? City { get; set; }

    public string? Province { get; set; }

    public string? Country { get; set; }

    public int? EstablishedYear { get; set; }

    public string? WebsiteUrl { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? LogoUrl { get; set; }

    public int? RankingNational { get; set; }

    public int? RankingInternational { get; set; }

    public string? Accreditation { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AdmissionCycle> AdmissionCycles { get; set; } = new List<AdmissionCycle>();

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

    public virtual ICollection<Mentor> Mentors { get; set; } = new List<Mentor>();

    public virtual ICollection<MeritFormula> MeritFormulas { get; set; } = new List<MeritFormula>();

    public virtual ICollection<Scholarship> Scholarships { get; set; } = new List<Scholarship>();

    public virtual ICollection<UniversityRepresentative> UniversityRepresentatives { get; set; } = new List<UniversityRepresentative>();
}
