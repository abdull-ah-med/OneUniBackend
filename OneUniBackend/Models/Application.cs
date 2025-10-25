using System;
using System.Collections.Generic;

namespace OneUniBackend.Models;

public partial class Application
{
    public Guid ApplicationId { get; set; }

    public Guid? UserId { get; set; }

    public Guid? UniversityId { get; set; }

    public Guid? ProgramId { get; set; }

    public Guid? CycleId { get; set; }

    public string? ApplicationNumber { get; set; }

    public DateTime? SubmissionDate { get; set; }

    public bool? ScholarshipApplied { get; set; }

    public bool? HostelRequired { get; set; }

    public bool? TransportRequired { get; set; }

    public decimal? CalculatedMerit { get; set; }

    public int? MeritPosition { get; set; }

    public string? FeeChallanNumber { get; set; }

    public decimal? FeePaidAmount { get; set; }

    public DateOnly? FeePaymentDate { get; set; }

    public bool? AdmissionOffered { get; set; }

    public DateOnly? OfferDate { get; set; }

    public DateOnly? OfferExpiresAt { get; set; }

    public string? RejectionReason { get; set; }

    public DateOnly? ScheduledSubmissionDate { get; set; }

    public bool? AutoSubmitted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual AdmissionCycle? Cycle { get; set; }

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual Programs? Programs { get; set; }

    public virtual University? University { get; set; }

    public virtual User? User { get; set; }
    public ApplicationStatus? ApplicationStatus { get; set; }
}
