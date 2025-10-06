using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OneUniBackend.Models;

public partial class OneuniContext : DbContext
{
    public OneuniContext(DbContextOptions<OneuniContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdmissionCycle> AdmissionCycles { get; set; }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<EducationalRecord> EducationalRecords { get; set; }

    public virtual DbSet<Mentor> Mentors { get; set; }

    public virtual DbSet<MentorshipSession> MentorshipSessions { get; set; }

    public virtual DbSet<MeritFormula> MeritFormulas { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Program> Programs { get; set; }

    public virtual DbSet<Scholarship> Scholarships { get; set; }

    public virtual DbSet<StudentProfile> StudentProfiles { get; set; }

    public virtual DbSet<TestScore> TestScores { get; set; }

    public virtual DbSet<University> Universities { get; set; }

    public virtual DbSet<UniversityRepresentative> UniversityRepresentatives { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Configuration is done in Program.cs via dependency injection
        // Do not override here unless running migrations without DI
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("application_status", new[] { "draft", "scheduled", "submitted", "under_review", "await_merit_list", "awaiting_fee_submission", "accepted", "rejected" })
            .HasPostgresEnum("document_type", new[] { "matric_certificate", "intermediate_certificate", "transcript", "cnic", "passport", "nicop", "b_form", "sports_certificate", "hafiz_certificate", "income_certificate", "domicile", "other" })
            .HasPostgresEnum("education_type", new[] { "matric", "intermediate", "a_levels", "o_levels", "equivalent" })
            .HasPostgresEnum("gender_type", new[] { "male", "female", "other", "prefer_not_to_say" })
            .HasPostgresEnum("guardian_relation", new[] { "father", "mother", "guardian", "other" })
            .HasPostgresEnum("id_document_type", new[] { "cnic", "nicop", "passport", "b_form" })
            .HasPostgresEnum("session_status", new[] { "scheduled", "completed", "cancelled", "no_show" })
            .HasPostgresEnum("session_type", new[] { "free", "paid" })
            .HasPostgresEnum("test_type", new[] { "NET", "ECAT", "MDCAT", "SAT", "IELTS", "TOEFL", "FAST", "LUMS", "other" })
            .HasPostgresEnum("user_role", new[] { "student", "mentor", "university_representative", "admin" })
            .HasPostgresEnum("verification_status", new[] { "pending", "verified", "rejected" })
            .HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<AdmissionCycle>(entity =>
        {
            entity.HasKey(e => e.CycleId).HasName("admission_cycles_pkey");

            entity.ToTable("admission_cycles");

            entity.Property(e => e.CycleId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("cycle_id");
            entity.Property(e => e.AcademicYear)
                .HasMaxLength(10)
                .HasColumnName("academic_year");
            entity.Property(e => e.ApplicationEndDate).HasColumnName("application_end_date");
            entity.Property(e => e.ApplicationStartDate).HasColumnName("application_start_date");
            entity.Property(e => e.ClassesStartDate).HasColumnName("classes_start_date");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.FeeSubmissionDeadline).HasColumnName("fee_submission_deadline");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.MeritListDate).HasColumnName("merit_list_date");
            entity.Property(e => e.SessionName)
                .HasMaxLength(50)
                .HasColumnName("session_name");
            entity.Property(e => e.TestDate).HasColumnName("test_date");
            entity.Property(e => e.UniversityId).HasColumnName("university_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.University).WithMany(p => p.AdmissionCycles)
                .HasForeignKey(d => d.UniversityId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("admission_cycles_university_id_fkey");
        });

        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.ApplicationId).HasName("applications_pkey");

            entity.ToTable("applications");

            entity.HasIndex(e => new { e.UniversityId, e.ProgramId }, "idx_applications_university_program");

            entity.HasIndex(e => e.UserId, "idx_applications_user_id");

            entity.Property(e => e.ApplicationId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("application_id");
            entity.Property(e => e.AdmissionOffered)
                .HasDefaultValue(false)
                .HasColumnName("admission_offered");
            entity.Property(e => e.ApplicationNumber)
                .HasMaxLength(50)
                .HasColumnName("application_number");
            entity.Property(e => e.Status)
                .HasColumnName("status");
            entity.Property(e => e.AutoSubmitted)
                .HasDefaultValue(false)
                .HasColumnName("auto_submitted");
            entity.Property(e => e.CalculatedMerit)
                .HasPrecision(8, 4)
                .HasColumnName("calculated_merit");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CycleId).HasColumnName("cycle_id");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.FeeChallanNumber)
                .HasMaxLength(100)
                .HasColumnName("fee_challan_number");
            entity.Property(e => e.FeePaidAmount)
                .HasPrecision(12, 2)
                .HasColumnName("fee_paid_amount");
            entity.Property(e => e.FeePaymentDate).HasColumnName("fee_payment_date");
            entity.Property(e => e.HostelRequired)
                .HasDefaultValue(false)
                .HasColumnName("hostel_required");
            entity.Property(e => e.MeritPosition).HasColumnName("merit_position");
            entity.Property(e => e.OfferDate).HasColumnName("offer_date");
            entity.Property(e => e.OfferExpiresAt).HasColumnName("offer_expires_at");
            entity.Property(e => e.ProgramId).HasColumnName("program_id");
            entity.Property(e => e.RejectionReason).HasColumnName("rejection_reason");
            entity.Property(e => e.ScheduledSubmissionDate).HasColumnName("scheduled_submission_date");
            entity.Property(e => e.ScholarshipApplied)
                .HasDefaultValue(false)
                .HasColumnName("scholarship_applied");
            entity.Property(e => e.SubmissionDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("submission_date");
            entity.Property(e => e.TransportRequired)
                .HasDefaultValue(false)
                .HasColumnName("transport_required");
            entity.Property(e => e.UniversityId).HasColumnName("university_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Cycle).WithMany(p => p.Applications)
                .HasForeignKey(d => d.CycleId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("applications_cycle_id_fkey");

            entity.HasOne(d => d.Program).WithMany(p => p.Applications)
                .HasForeignKey(d => d.ProgramId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("applications_program_id_fkey");

            entity.HasOne(d => d.University).WithMany(p => p.Applications)
                .HasForeignKey(d => d.UniversityId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("applications_university_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Applications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("applications_user_id_fkey");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("audit_logs_pkey");

            entity.ToTable("audit_logs");

            entity.Property(e => e.LogId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("log_id");
            entity.Property(e => e.Action)
                .HasMaxLength(100)
                .HasColumnName("action");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IpAddress).HasColumnName("ip_address");
            entity.Property(e => e.NewValues)
                .HasColumnType("jsonb")
                .HasColumnName("new_values");
            entity.Property(e => e.OldValues)
                .HasColumnType("jsonb")
                .HasColumnName("old_values");
            entity.Property(e => e.RecordId).HasColumnName("record_id");
            entity.Property(e => e.TableName)
                .HasMaxLength(100)
                .HasColumnName("table_name");
            entity.Property(e => e.UserAgent).HasColumnName("user_agent");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("audit_logs_user_id_fkey");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("departments_pkey");

            entity.ToTable("departments");

            entity.HasIndex(e => e.UniversityId, "idx_departments_university_id");

            entity.Property(e => e.DepartmentId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("department_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.HeadOfDepartment)
                .HasMaxLength(255)
                .HasColumnName("head_of_department");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.ShortName)
                .HasMaxLength(20)
                .HasColumnName("short_name");
            entity.Property(e => e.UniversityId).HasColumnName("university_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.University).WithMany(p => p.Departments)
                .HasForeignKey(d => d.UniversityId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("departments_university_id_fkey");
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("documents_pkey");

            entity.ToTable("documents");

            entity.HasIndex(e => e.ApplicationId, "idx_documents_application_id");

            entity.HasIndex(e => e.UserId, "idx_documents_user_id");

            entity.Property(e => e.DocumentId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("document_id");
            entity.Property(e => e.ApplicationId).HasColumnName("application_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DisplayOrder)
                .HasDefaultValue(0)
                .HasColumnName("display_order");
            entity.Property(e => e.DocumentName)
                .HasMaxLength(255)
                .HasColumnName("document_name");
            entity.Property(e => e.DocumentType)
                .HasColumnName("document_type");
            entity.Property(e => e.VerificationStatus)
                .HasColumnName("verification_status");
            entity.Property(e => e.FilePath)
                .HasMaxLength(500)
                .HasColumnName("file_path");
            entity.Property(e => e.FileSize).HasColumnName("file_size");
            entity.Property(e => e.IsRequired)
                .HasDefaultValue(false)
                .HasColumnName("is_required");
            entity.Property(e => e.MimeType)
                .HasMaxLength(100)
                .HasColumnName("mime_type");
            entity.Property(e => e.RejectionReason).HasColumnName("rejection_reason");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VerifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("verified_at");
            entity.Property(e => e.VerifiedBy).HasColumnName("verified_by");

            entity.HasOne(d => d.Application).WithMany(p => p.Documents)
                .HasForeignKey(d => d.ApplicationId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("documents_application_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.DocumentUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("documents_user_id_fkey");

            entity.HasOne(d => d.VerifiedByNavigation).WithMany(p => p.DocumentVerifiedByNavigations)
                .HasForeignKey(d => d.VerifiedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("documents_verified_by_fkey");
        });

        modelBuilder.Entity<EducationalRecord>(entity =>
        {
            entity.HasKey(e => e.RecordId).HasName("educational_records_pkey");

            entity.ToTable("educational_records");

            entity.HasIndex(e => e.UserId, "idx_educational_records_user_id");

            entity.Property(e => e.RecordId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("record_id");
            entity.Property(e => e.EducationType)
                .HasColumnName("education_type");
            entity.Property(e => e.BoardUniversity)
                .HasMaxLength(255)
                .HasColumnName("board_university");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Grade)
                .HasMaxLength(10)
                .HasColumnName("grade");
            entity.Property(e => e.InstitutionName)
                .HasMaxLength(255)
                .HasColumnName("institution_name");
            entity.Property(e => e.IsResultAwaited)
                .HasDefaultValue(false)
                .HasColumnName("is_result_awaited");
            entity.Property(e => e.ObtainedMarks).HasColumnName("obtained_marks");
            entity.Property(e => e.Percentage)
                .HasPrecision(5, 2)
                .HasColumnName("percentage");
            entity.Property(e => e.RollNumber)
                .HasMaxLength(50)
                .HasColumnName("roll_number");
            entity.Property(e => e.TotalMarks).HasColumnName("total_marks");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.YearOfCompletion).HasColumnName("year_of_completion");

            entity.HasOne(d => d.User).WithMany(p => p.EducationalRecords)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("educational_records_user_id_fkey");
        });

        modelBuilder.Entity<Mentor>(entity =>
        {
            entity.HasKey(e => e.MentorId).HasName("mentors_pkey");

            entity.ToTable("mentors");

            entity.Property(e => e.MentorId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("mentor_id");
            entity.Property(e => e.AvailabilityHours)
                .HasColumnType("jsonb")
                .HasColumnName("availability_hours");
            entity.Property(e => e.AverageRating)
                .HasPrecision(3, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("average_rating");
            entity.Property(e => e.Bio).HasColumnName("bio");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CurrentInstitution)
                .HasMaxLength(255)
                .HasColumnName("current_institution");
            entity.Property(e => e.Designation)
                .HasMaxLength(255)
                .HasColumnName("designation");
            entity.Property(e => e.ExperienceYears).HasColumnName("experience_years");
            entity.Property(e => e.FieldOfStudy)
                .HasMaxLength(255)
                .HasColumnName("field_of_study");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.GraduationYear).HasColumnName("graduation_year");
            entity.Property(e => e.HourlyRate)
                .HasPrecision(8, 2)
                .HasColumnName("hourly_rate");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LinkedinUrl)
                .HasMaxLength(500)
                .HasColumnName("linkedin_url");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.ProfilePictureUrl)
                .HasMaxLength(500)
                .HasColumnName("profile_picture_url");
            entity.Property(e => e.Specializations).HasColumnName("specializations");
            entity.Property(e => e.TotalSessions)
                .HasDefaultValue(0)
                .HasColumnName("total_sessions");
            entity.Property(e => e.UniversityEmail)
                .HasMaxLength(255)
                .HasColumnName("university_email");
            entity.Property(e => e.UniversityId).HasColumnName("university_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.University).WithMany(p => p.Mentors)
                .HasForeignKey(d => d.UniversityId)
                .HasConstraintName("mentors_university_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Mentors)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("mentors_user_id_fkey");
        });

        modelBuilder.Entity<MentorshipSession>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("mentorship_sessions_pkey");

            entity.ToTable("mentorship_sessions");

            entity.HasIndex(e => e.MentorId, "idx_mentorship_sessions_mentor_id");

            entity.HasIndex(e => e.StudentId, "idx_mentorship_sessions_student_id");

            entity.Property(e => e.SessionId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("session_id");
            entity.Property(e => e.SessionType)
                .HasColumnName("session_type");
            entity.Property(e => e.SessionStatus)
                .HasColumnName("session_status");
            entity.Property(e => e.ActualEndTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("actual_end_time");
            entity.Property(e => e.ActualStartTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("actual_start_time");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DurationMinutes)
                .HasDefaultValue(60)
                .HasColumnName("duration_minutes");
            entity.Property(e => e.FeeAmount)
                .HasPrecision(8, 2)
                .HasColumnName("fee_amount");
            entity.Property(e => e.MentorFeedback).HasColumnName("mentor_feedback");
            entity.Property(e => e.MentorId).HasColumnName("mentor_id");
            entity.Property(e => e.MentorRating).HasColumnName("mentor_rating");
            entity.Property(e => e.PaymentReference)
                .HasMaxLength(100)
                .HasColumnName("payment_reference");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .HasColumnName("payment_status");
            entity.Property(e => e.ScheduledAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("scheduled_at");
            entity.Property(e => e.SessionNotes).HasColumnName("session_notes");
            entity.Property(e => e.StudentFeedback).HasColumnName("student_feedback");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.StudentRating).HasColumnName("student_rating");
            entity.Property(e => e.Topic)
                .HasMaxLength(255)
                .HasColumnName("topic");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Mentor).WithMany(p => p.MentorshipSessions)
                .HasForeignKey(d => d.MentorId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("mentorship_sessions_mentor_id_fkey");

            entity.HasOne(d => d.Student).WithMany(p => p.MentorshipSessions)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("mentorship_sessions_student_id_fkey");
        });

        modelBuilder.Entity<MeritFormula>(entity =>
        {
            entity.HasKey(e => e.FormulaId).HasName("merit_formulas_pkey");

            entity.ToTable("merit_formulas");

            entity.Property(e => e.FormulaId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("formula_id");
            entity.Property(e => e.AcademicYear)
                .HasMaxLength(10)
                .HasColumnName("academic_year");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IntermediateWeightage)
                .HasPrecision(5, 2)
                .HasColumnName("intermediate_weightage");
            entity.Property(e => e.InterviewWeightage)
                .HasPrecision(5, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("interview_weightage");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.MatricWeightage)
                .HasPrecision(5, 2)
                .HasColumnName("matric_weightage");
            entity.Property(e => e.MinimumIntermediatePercentage)
                .HasPrecision(5, 2)
                .HasColumnName("minimum_intermediate_percentage");
            entity.Property(e => e.MinimumMatricPercentage)
                .HasPrecision(5, 2)
                .HasColumnName("minimum_matric_percentage");
            entity.Property(e => e.MinimumTestScore)
                .HasPrecision(5, 2)
                .HasColumnName("minimum_test_score");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.ProgramId).HasColumnName("program_id");
            entity.Property(e => e.TestWeightage)
                .HasPrecision(5, 2)
                .HasColumnName("test_weightage");
            entity.Property(e => e.UniversityId).HasColumnName("university_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Program).WithMany(p => p.MeritFormulas)
                .HasForeignKey(d => d.ProgramId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("merit_formulas_program_id_fkey");

            entity.HasOne(d => d.University).WithMany(p => p.MeritFormulas)
                .HasForeignKey(d => d.UniversityId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("merit_formulas_university_id_fkey");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("notifications_pkey");

            entity.ToTable("notifications");

            entity.HasIndex(e => e.UserId, "idx_notifications_user_id");

            entity.Property(e => e.NotificationId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("notification_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.ReadAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("read_at");
            entity.Property(e => e.RelatedApplicationId).HasColumnName("related_application_id");
            entity.Property(e => e.RelatedSessionId).HasColumnName("related_session_id");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.RelatedApplication).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.RelatedApplicationId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("notifications_related_application_id_fkey");

            entity.HasOne(d => d.RelatedSession).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.RelatedSessionId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("notifications_related_session_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("notifications_user_id_fkey");
        });

        modelBuilder.Entity<Program>(entity =>
        {
            entity.HasKey(e => e.ProgramId).HasName("programs_pkey");

            entity.ToTable("programs");

            entity.HasIndex(e => e.DepartmentId, "idx_programs_department_id");

            entity.Property(e => e.ProgramId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("program_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DegreeType)
                .HasMaxLength(50)
                .HasDefaultValueSql("'BS'::character varying")
                .HasColumnName("degree_type");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DurationYears)
                .HasDefaultValue(4)
                .HasColumnName("duration_years");
            entity.Property(e => e.EligibilityCriteria).HasColumnName("eligibility_criteria");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.TotalCreditHours).HasColumnName("total_credit_hours");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Department).WithMany(p => p.Programs)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("programs_department_id_fkey");
        });

        modelBuilder.Entity<Scholarship>(entity =>
        {
            entity.HasKey(e => e.ScholarshipId).HasName("scholarships_pkey");

            entity.ToTable("scholarships");

            entity.Property(e => e.ScholarshipId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("scholarship_id");
            entity.Property(e => e.AcademicYear)
                .HasMaxLength(10)
                .HasColumnName("academic_year");
            entity.Property(e => e.AdditionalBenefits).HasColumnName("additional_benefits");
            entity.Property(e => e.ApplicationDeadline).HasColumnName("application_deadline");
            entity.Property(e => e.CoverageAmount)
                .HasPrecision(12, 2)
                .HasColumnName("coverage_amount");
            entity.Property(e => e.CoveragePercentage)
                .HasPrecision(5, 2)
                .HasColumnName("coverage_percentage");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IncomeCriteria)
                .HasPrecision(12, 2)
                .HasColumnName("income_criteria");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.MinimumMerit)
                .HasPrecision(8, 4)
                .HasColumnName("minimum_merit");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.RegionSpecific)
                .HasMaxLength(100)
                .HasColumnName("region_specific");
            entity.Property(e => e.UniversityId).HasColumnName("university_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.University).WithMany(p => p.Scholarships)
                .HasForeignKey(d => d.UniversityId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("scholarships_university_id_fkey");
        });

        modelBuilder.Entity<StudentProfile>(entity =>
        {
            entity.HasKey(e => e.ProfileId).HasName("student_profiles_pkey");

            entity.ToTable("student_profiles");

            entity.HasIndex(e => e.UserId, "idx_student_profiles_user_id");

            entity.Property(e => e.ProfileId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("profile_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.Cnic)
                .HasMaxLength(15)
                .HasColumnName("cnic");
            entity.Property(e => e.CompletionPercentage)
                .HasDefaultValue(0)
                .HasColumnName("completion_percentage");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender)
                .HasColumnName("gender");
            entity.Property(e => e.IdDocumentType)
                .HasColumnName("id_document_type");
            entity.Property(e => e.GuardianName)
                .HasMaxLength(255)
                .HasColumnName("guardian_name");
            entity.Property(e => e.GuardianRelation)
                .HasColumnName("guardian_relation");
            entity.Property(e => e.HostelPriority)
                .HasDefaultValue(false)
                .HasColumnName("hostel_priority");
            entity.Property(e => e.HouseholdIncome)
                .HasPrecision(12, 2)
                .HasColumnName("household_income");
            entity.Property(e => e.IsDisabled)
                .HasDefaultValue(false)
                .HasColumnName("is_disabled");
            entity.Property(e => e.IsHafizQuran)
                .HasDefaultValue(false)
                .HasColumnName("is_hafiz_quran");
            entity.Property(e => e.NicopNumber)
                .HasMaxLength(20)
                .HasColumnName("nicop_number");
            entity.Property(e => e.PassportNumber)
                .HasMaxLength(20)
                .HasColumnName("passport_number");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.PreferredAdmissionCity)
                .HasMaxLength(100)
                .HasColumnName("preferred_admission_city");
            entity.Property(e => e.ProfileCompleted)
                .HasDefaultValue(false)
                .HasColumnName("profile_completed");
            entity.Property(e => e.ProfilePictureUrl)
                .HasMaxLength(500)
                .HasColumnName("profile_picture_url");
            entity.Property(e => e.ScholarshipPriority)
                .HasDefaultValue(false)
                .HasColumnName("scholarship_priority");
            entity.Property(e => e.Sports)
                .HasMaxLength(100)
                .HasColumnName("sports");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.StudentProfiles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("student_profiles_user_id_fkey");
        });

        modelBuilder.Entity<TestScore>(entity =>
        {
            entity.HasKey(e => e.ScoreId).HasName("test_scores_pkey");

            entity.ToTable("test_scores");

            entity.HasIndex(e => e.UserId, "idx_test_scores_user_id");

            entity.Property(e => e.ScoreId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("score_id");
            entity.Property(e => e.TestType)
                .HasColumnName("test_type");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ObtainedMarks).HasColumnName("obtained_marks");
            entity.Property(e => e.Percentage)
                .HasPrecision(5, 2)
                .HasColumnName("percentage");
            entity.Property(e => e.RollNumber)
                .HasMaxLength(50)
                .HasColumnName("roll_number");
            entity.Property(e => e.TestDate).HasColumnName("test_date");
            entity.Property(e => e.TestName)
                .HasMaxLength(255)
                .HasColumnName("test_name");
            entity.Property(e => e.TotalMarks).HasColumnName("total_marks");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Year).HasColumnName("year");

            entity.HasOne(d => d.User).WithMany(p => p.TestScores)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("test_scores_user_id_fkey");
        });

        modelBuilder.Entity<University>(entity =>
        {
            entity.HasKey(e => e.UniversityId).HasName("universities_pkey");

            entity.ToTable("universities");

            entity.Property(e => e.UniversityId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("university_id");
            entity.Property(e => e.Accreditation).HasColumnName("accreditation");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .HasDefaultValueSql("'Pakistan'::character varying")
                .HasColumnName("country");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.EstablishedYear).HasColumnName("established_year");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LogoUrl)
                .HasMaxLength(500)
                .HasColumnName("logo_url");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Province)
                .HasMaxLength(50)
                .HasColumnName("province");
            entity.Property(e => e.RankingInternational).HasColumnName("ranking_international");
            entity.Property(e => e.RankingNational).HasColumnName("ranking_national");
            entity.Property(e => e.ShortName)
                .HasMaxLength(20)
                .HasColumnName("short_name");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.WebsiteUrl)
                .HasMaxLength(500)
                .HasColumnName("website_url");
        });

        modelBuilder.Entity<UniversityRepresentative>(entity =>
        {
            entity.HasKey(e => e.RepId).HasName("university_representatives_pkey");

            entity.ToTable("university_representatives");

            entity.Property(e => e.RepId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("rep_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Department)
                .HasMaxLength(255)
                .HasColumnName("department");
            entity.Property(e => e.Designation)
                .HasMaxLength(255)
                .HasColumnName("designation");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsOfficial)
                .HasDefaultValue(false)
                .HasColumnName("is_official");
            entity.Property(e => e.OfficeAddress).HasColumnName("office_address");
            entity.Property(e => e.Permissions)
                .HasColumnType("jsonb")
                .HasColumnName("permissions");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.UniversityId).HasColumnName("university_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VerificationDocumentUrl)
                .HasMaxLength(500)
                .HasColumnName("verification_document_url");

            entity.HasOne(d => d.University).WithMany(p => p.UniversityRepresentatives)
                .HasForeignKey(d => d.UniversityId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("university_representatives_university_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UniversityRepresentatives)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("university_representatives_user_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "idx_users_email");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.UserId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("user_id");
            entity.Property(e => e.Role)
                .HasColumnName("role");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsVerified)
                .HasDefaultValue(false)
                .HasColumnName("is_verified");
            entity.Property(e => e.LastLogin)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_login");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.PasswordResetExpires)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("password_reset_expires");
            entity.Property(e => e.PasswordResetToken)
                .HasMaxLength(255)
                .HasColumnName("password_reset_token");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.VerificationToken)
                .HasMaxLength(255)
                .HasColumnName("verification_token");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
