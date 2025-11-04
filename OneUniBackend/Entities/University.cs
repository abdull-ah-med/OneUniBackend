using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OneUniBackend.Entities;

[Table("universities")]
public partial class University
{
    [Key]
    [Column("university_id")]
    public Guid UniversityId { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Column("short_name")]
    [StringLength(20)]
    public string? ShortName { get; set; }

    [Column("type")]
    [StringLength(50)]
    public string? Type { get; set; }

    [Column("city")]
    [StringLength(100)]
    public string? City { get; set; }

    [Column("province")]
    [StringLength(50)]
    public string? Province { get; set; }

    [Column("country")]
    [StringLength(50)]
    public string? Country { get; set; }

    [Column("established_year")]
    public int? EstablishedYear { get; set; }

    [Column("website_url")]
    [StringLength(500)]
    public string? WebsiteUrl { get; set; }

    [Column("email")]
    [StringLength(255)]
    public string? Email { get; set; }

    [Column("phone")]
    [StringLength(20)]
    public string? Phone { get; set; }

    [Column("address")]
    public string? Address { get; set; }

    [Column("logo_url")]
    [StringLength(500)]
    public string? LogoUrl { get; set; }

    [Column("ranking_national")]
    public int? RankingNational { get; set; }

    [Column("ranking_international")]
    public int? RankingInternational { get; set; }

    [Column("accreditation")]
    public string? Accreditation { get; set; }

    [Column("is_active")]
    public bool? IsActive { get; set; }

    [Column("created_at", TypeName = "timestamp with time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp with time zone")]
    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("University")]
    public virtual ICollection<AdmissionCycle> AdmissionCycles { get; set; } = new List<AdmissionCycle>();

    [InverseProperty("University")]
    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    [InverseProperty("University")]
    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

    [InverseProperty("University")]
    public virtual ICollection<Mentor> Mentors { get; set; } = new List<Mentor>();

    [InverseProperty("University")]
    public virtual ICollection<MeritFormula> MeritFormulas { get; set; } = new List<MeritFormula>();

    [InverseProperty("University")]
    public virtual ICollection<Scholarship> Scholarships { get; set; } = new List<Scholarship>();

    [InverseProperty("University")]
    public virtual ICollection<UniversityRepresentative> UniversityRepresentatives { get; set; } = new List<UniversityRepresentative>();
}
