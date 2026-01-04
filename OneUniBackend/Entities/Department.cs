using System;
using System.Collections.Generic;

namespace OneUniBackend.Entities;

public partial class Department
{
    public Guid DepartmentId { get; set; }

    public Guid? UniversityId { get; set; }

    public string Name { get; set; } = null!;

    public string? ShortName { get; set; }

    public string? Description { get; set; }

    public string? HeadOfDepartment { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Program> Programs { get; set; } = new List<Program>();

    public virtual University? University { get; set; }
}
