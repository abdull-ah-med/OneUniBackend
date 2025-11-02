using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using OneUni.Enums;
using ProgramEntity = OneUni.Entities.Program;

namespace OneUni.Entities;

[Table("applications")]
[Index("UniversityId", "ProgramId", Name = "idx_applications_university_program")]
[Index("UserId", Name = "idx_applications_user_id")]
public partial class Application
{
    [Key]
    [Column("application_id")]
    public Guid ApplicationId { get; set; }

    [Column("user_id")]
    public Guid? UserId { get; set; }

    [Column("university_id")]
    public Guid? UniversityId { get; set; }

    [Column("program_id")]
    public Guid? ProgramId { get; set; }

    [Column("cycle_id")]
    public Guid? CycleId { get; set; }

    [Column("application_number")]
    [StringLength(50)]
    public string? ApplicationNumber { get; set; }

    [Column("submission_date", TypeName = "timestamp without time zone")]
    public DateTime? SubmissionDate { get; set; }

    [Column("scholarship_applied")]
    public bool? ScholarshipApplied { get; set; }

    [Column("hostel_required")]
    public bool? HostelRequired { get; set; }

    [Column("transport_required")]
    public bool? TransportRequired { get; set; }

    [Column("calculated_merit")]
    [Precision(8, 4)]
    public decimal? CalculatedMerit { get; set; }

    [Column("merit_position")]
    public int? MeritPosition { get; set; }

    [Column("fee_challan_number")]
    [StringLength(100)]
    public string? FeeChallanNumber { get; set; }

    [Column("fee_paid_amount")]
    [Precision(12, 2)]
    public decimal? FeePaidAmount { get; set; }

    [Column("fee_payment_date")]
    public DateOnly? FeePaymentDate { get; set; }

    [Column("admission_offered")]
    public bool? AdmissionOffered { get; set; }

    [Column("offer_date")]
    public DateOnly? OfferDate { get; set; }

    [Column("offer_expires_at")]
    public DateOnly? OfferExpiresAt { get; set; }

    [Column("rejection_reason")]
    public string? RejectionReason { get; set; }

    [Column("scheduled_submission_date")]
    public DateOnly? ScheduledSubmissionDate { get; set; }

    [Column("auto_submitted")]
    public bool? AutoSubmitted { get; set; }

    [Column("application_status")]
    public ApplicationStatus Status { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [Column("deleted_at", TypeName = "timestamp without time zone")]
    public DateTime? DeletedAt { get; set; }

    [ForeignKey("CycleId")]
    [InverseProperty("Applications")]
    public virtual AdmissionCycle? Cycle { get; set; }

    [InverseProperty("Application")]
    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    [InverseProperty("RelatedApplication")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [ForeignKey("ProgramId")]
    [InverseProperty("Applications")]
    public virtual ProgramEntity? Program { get; set; }

    [ForeignKey("UniversityId")]
    [InverseProperty("Applications")]
    public virtual University? University { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Applications")]
    public virtual User? User { get; set; }
}
