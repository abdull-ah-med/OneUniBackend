using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
namespace OneUniBackend.Data;

using OneUniBackend.Entities;
using ProgramEntity = OneUniBackend.Entities.Program;
public partial class OneUniDbContext : DbContext
{
    public OneUniDbContext()
    {
    }

    public OneUniDbContext(DbContextOptions<OneUniDbContext> options)
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

    public virtual DbSet<UserLogin> UserLogins { get; set; }

    public virtual DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=oneuni;Username=postgres;Password=");

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

            entity.Property(e => e.CycleId).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'UTC'::text)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'UTC'::text)");

            entity.HasOne(d => d.University).WithMany(p => p.AdmissionCycles)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("admission_cycles_university_id_fkey");
        });

        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.ApplicationId).HasName("applications_pkey");

            entity.Property(e => e.ApplicationId).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.Status).HasColumnType("application_status");
            entity.Property(e => e.AdmissionOffered).HasDefaultValue(false);
            entity.Property(e => e.AutoSubmitted).HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'UTC'::text)");
            entity.Property(e => e.HostelRequired).HasDefaultValue(false);
            entity.Property(e => e.ScholarshipApplied).HasDefaultValue(false);
            entity.Property(e => e.TransportRequired).HasDefaultValue(false);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'UTC'::text)");

            entity.HasOne(d => d.Cycle).WithMany(p => p.Applications)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("applications_cycle_id_fkey");

            entity.HasOne(d => d.Program).WithMany(p => p.Applications)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("applications_program_id_fkey");

            entity.HasOne(d => d.University).WithMany(p => p.Applications)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("applications_university_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Applications)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("applications_user_id_fkey");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("audit_logs_pkey");

            entity.Property(e => e.LogId).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'UTC'::text)");

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("audit_logs_user_id_fkey");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("departments_pkey");

            entity.Property(e => e.DepartmentId).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'UTC'::text)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'UTC'::text)");

            entity.HasOne(d => d.University).WithMany(p => p.Departments)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("departments_university_id_fkey");
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("documents_pkey");

            entity.Property(e => e.DocumentId).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.DocumentType).HasColumnType("document_type");
            entity.Property(e => e.VerificationStatus).HasColumnType("verification_status");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'UTC'::text)");
            entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
            entity.Property(e => e.IsRequired).HasDefaultValue(false);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'UTC'::text)");

            entity.HasOne(d => d.Application).WithMany(p => p.Documents)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("documents_application_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.DocumentUsers)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("documents_user_id_fkey");

            entity.HasOne(d => d.VerifiedByNavigation).WithMany(p => p.DocumentVerifiedByNavigations)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("documents_verified_by_fkey");
        });

        modelBuilder.Entity<EducationalRecord>(entity =>
        {
            entity.HasKey(e => e.RecordId).HasName("educational_records_pkey");

            entity.Property(e => e.RecordId).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.EducationType).HasColumnType("education_type");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'UTC'::text)");
            entity.Property(e => e.IsResultAwaited).HasDefaultValue(false);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'UTC'::text)");

            entity.HasOne(d => d.User).WithMany(p => p.EducationalRecords)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("educational_records_user_id_fkey");
        });

        modelBuilder.Entity<Mentor>(entity =>
        {
            entity.HasKey(e => e.MentorId).HasName("mentors_pkey");

            entity.Property(e => e.MentorId).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.VerificationStatus).HasColumnType("verification_status");
            entity.Property(e => e.AverageRating).HasDefaultValueSql("0");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.TotalSessions).HasDefaultValue(0);

            entity.HasOne(d => d.University).WithMany(p => p.Mentors).HasConstraintName("mentors_university_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Mentors)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("mentors_user_id_fkey");
        });

        modelBuilder.Entity<MentorshipSession>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("mentorship_sessions_pkey");

            entity.Property(e => e.SessionId).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.SessionType).HasColumnType("session_type");
            entity.Property(e => e.SessionStatus).HasColumnType("session_status");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'UTC'::text)");
            entity.Property(e => e.DurationMinutes).HasDefaultValue(60);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'UTC'::text)");

            entity.HasOne(d => d.Mentor).WithMany(p => p.MentorshipSessions)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("mentorship_sessions_mentor_id_fkey");

            entity.HasOne(d => d.Student).WithMany(p => p.MentorshipSessions)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("mentorship_sessions_student_id_fkey");
        });

        modelBuilder.Entity<MeritFormula>(entity =>
        {
            entity.HasKey(e => e.FormulaId).HasName("merit_formulas_pkey");

            entity.Property(e => e.FormulaId).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'UTC'::text)");
            entity.Property(e => e.InterviewWeightage).HasDefaultValueSql("0");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'UTC'::text)");

            entity.HasOne(d => d.Program).WithMany(p => p.MeritFormulas)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("merit_formulas_program_id_fkey");

            entity.HasOne(d => d.University).WithMany(p => p.MeritFormulas)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("merit_formulas_university_id_fkey");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("notifications_pkey");

            entity.Property(e => e.NotificationId).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'UTC'::text)");

            entity.HasOne(d => d.RelatedApplication).WithMany(p => p.Notifications)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("notifications_related_application_id_fkey");

            entity.HasOne(d => d.RelatedSession).WithMany(p => p.Notifications)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("notifications_related_session_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("notifications_user_id_fkey");
        });

        modelBuilder.Entity<ProgramEntity>(entity =>
        {
            entity.HasKey(e => e.ProgramId).HasName("programs_pkey");

            entity.Property(e => e.ProgramId).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'UTC'::text)");
            entity.Property(e => e.DegreeType).HasDefaultValueSql("'BS'::character varying");
            entity.Property(e => e.DurationYears).HasDefaultValue(4);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'UTC'::text)");

            entity.HasOne(d => d.Department).WithMany(p => p.Program)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("programs_department_id_fkey");
        });

        modelBuilder.Entity<Scholarship>(entity =>
        {
            entity.HasKey(e => e.ScholarshipId).HasName("scholarships_pkey");

            entity.Property(e => e.ScholarshipId).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'UTC'::text)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'UTC'::text)");

            entity.HasOne(d => d.University).WithMany(p => p.Scholarships)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("scholarships_university_id_fkey");
        });

        modelBuilder.Entity<StudentProfile>(entity =>
        {
            entity.HasKey(e => e.ProfileId).HasName("student_profiles_pkey");

            entity.Property(e => e.ProfileId).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.Gender).HasColumnType("gender_type");
            entity.Property(e => e.IdDocumentType).HasColumnType("id_document_type");
            entity.Property(e => e.GuardianRelation).HasColumnType("guardian_relation");
            entity.Property(e => e.CompletionPercentage).HasDefaultValue(0);
            entity.Property(e => e.HostelPriority).HasDefaultValue(false);
            entity.Property(e => e.IsDisabled).HasDefaultValue(false);
            entity.Property(e => e.IsHafizQuran).HasDefaultValue(false);
            entity.Property(e => e.ProfileCompleted).HasDefaultValue(false);
            entity.Property(e => e.ScholarshipPriority).HasDefaultValue(false);

            entity.HasOne(d => d.User).WithMany(p => p.StudentProfiles)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("student_profiles_user_id_fkey");
        });

        modelBuilder.Entity<TestScore>(entity =>
        {
            entity.HasKey(e => e.ScoreId).HasName("test_scores_pkey");

            entity.Property(e => e.ScoreId).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.TestType).HasColumnType("test_type");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'UTC'::text)");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'UTC'::text)");

            entity.HasOne(d => d.User).WithMany(p => p.TestScores)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("test_scores_user_id_fkey");
        });

        modelBuilder.Entity<University>(entity =>
        {
            entity.HasKey(e => e.UniversityId).HasName("universities_pkey");

            entity.Property(e => e.UniversityId).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.Country).HasDefaultValueSql("'Pakistan'::character varying");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'UTC'::text)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'UTC'::text)");
        });

        modelBuilder.Entity<UniversityRepresentative>(entity =>
        {
            entity.HasKey(e => e.RepId).HasName("university_representatives_pkey");

            entity.Property(e => e.RepId).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.VerificationStatus).HasColumnType("verification_status");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsOfficial).HasDefaultValue(false);

            entity.HasOne(d => d.University).WithMany(p => p.UniversityRepresentatives)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("university_representatives_university_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UniversityRepresentatives)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("university_representatives_user_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.Property(e => e.UserId).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.Role).HasColumnType("user_role");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'UTC'::text)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsVerified).HasDefaultValue(false);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'UTC'::text)");
        });

        modelBuilder.Entity<UserLogin>(entity =>
        {
            entity.HasKey(e => new { e.Loginprovider, e.Providerkey }).HasName("pk_user_logins");

            entity.HasOne(d => d.User).WithMany(p => p.UserLogins).HasConstraintName("fk_user_logins_users_userid");
        });

        modelBuilder.Entity<UserRefreshToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("user_refresh_tokens_pkey");

            entity.Property(e => e.TokenId).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'UTC'::text)");
            entity.Property(e => e.IsRevoked).HasDefaultValue(false);

            entity.HasOne(d => d.User).WithMany(p => p.UserRefreshTokens).HasConstraintName("user_refresh_tokens_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
