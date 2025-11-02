using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using ProgramEntity = OneUni.Entities.Program;

namespace OneUni.Entities;

[Table("departments")]
[Index("UniversityId", Name = "idx_departments_university_id")]
public partial class Department
{
    [Key]
    [Column("department_id")]
    public Guid DepartmentId { get; set; }

    [Column("university_id")]
    public Guid? UniversityId { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Column("short_name")]
    [StringLength(20)]
    public string? ShortName { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("head_of_department")]
    [StringLength(255)]
    public string? HeadOfDepartment { get; set; }

    [Column("email")]
    [StringLength(255)]
    public string? Email { get; set; }

    [Column("phone")]
    [StringLength(20)]
    public string? Phone { get; set; }

    [Column("is_active")]
    public bool? IsActive { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("Department")]
    public virtual ICollection<ProgramEntity> Program { get; set; } = new List<ProgramEntity>();

    [ForeignKey("UniversityId")]
    [InverseProperty("Departments")]
    public virtual University? University { get; set; }
}
