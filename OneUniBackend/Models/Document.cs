using System;
using System.Collections.Generic;

namespace OneUniBackend.Models;

public partial class Document
{
    public Guid DocumentId { get; set; }

    public Guid? UserId { get; set; }

    public Guid? ApplicationId { get; set; }

    public string DocumentName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public int? FileSize { get; set; }

    public string? MimeType { get; set; }

    public Guid? VerifiedBy { get; set; }

    public DateTime? VerifiedAt { get; set; }

    public string? RejectionReason { get; set; }

    public bool? IsRequired { get; set; }

    public int? DisplayOrder { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Application? Application { get; set; }

    public virtual User? User { get; set; }

    public virtual User? VerifiedByNavigation { get; set; }
}
