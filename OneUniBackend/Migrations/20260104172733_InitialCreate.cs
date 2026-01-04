using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using OneUniBackend.Enums;

#nullable disable

namespace OneUniBackend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:application_status", "draft,scheduled,submitted,under_review,await_merit_list,awaiting_fee_submission,accepted,rejected")
                .Annotation("Npgsql:Enum:document_type", "matric_certificate,intermediate_certificate,transcript,cnic,passport,nicop,b_form,sports_certificate,hafiz_certificate,income_certificate,domicile,other")
                .Annotation("Npgsql:Enum:education_type", "matric,intermediate,a_levels,o_levels,equivalent")
                .Annotation("Npgsql:Enum:gender_type", "male,female,other,prefer_not_to_say")
                .Annotation("Npgsql:Enum:guardian_relation", "father,mother,guardian,other")
                .Annotation("Npgsql:Enum:id_document_type", "cnic,nicop,passport,b_form")
                .Annotation("Npgsql:Enum:session_status", "scheduled,completed,cancelled,no_show")
                .Annotation("Npgsql:Enum:session_type", "free,paid")
                .Annotation("Npgsql:Enum:test_type", "NET,ECAT,MDCAT,SAT,IELTS,TOEFL,FAST,LUMS,other")
                .Annotation("Npgsql:Enum:user_role", "student,mentor,university_representative,admin")
                .Annotation("Npgsql:Enum:verification_status", "pending,verified,rejected");

            migrationBuilder.CreateTable(
                name: "universities",
                columns: table => new
                {
                    university_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    short_name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    province = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    country = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "'Pakistan'::character varying"),
                    established_year = table.Column<int>(type: "integer", nullable: true),
                    website_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    address = table.Column<string>(type: "text", nullable: true),
                    logo_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ranking_national = table.Column<int>(type: "integer", nullable: true),
                    ranking_international = table.Column<int>(type: "integer", nullable: true),
                    accreditation = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("universities_pkey", x => x.university_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    full_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    role = table.Column<UserRole>(type: "user_role", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    is_verified = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    verification_token = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    password_reset_token = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    password_reset_expires = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_login = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)"),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("users_pkey", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "admission_cycles",
                columns: table => new
                {
                    cycle_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    university_id = table.Column<Guid>(type: "uuid", nullable: true),
                    academic_year = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    session_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    application_start_date = table.Column<DateOnly>(type: "date", nullable: true),
                    application_end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    test_date = table.Column<DateOnly>(type: "date", nullable: true),
                    merit_list_date = table.Column<DateOnly>(type: "date", nullable: true),
                    fee_submission_deadline = table.Column<DateOnly>(type: "date", nullable: true),
                    classes_start_date = table.Column<DateOnly>(type: "date", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("admission_cycles_pkey", x => x.cycle_id);
                    table.ForeignKey(
                        name: "admission_cycles_university_id_fkey",
                        column: x => x.university_id,
                        principalTable: "universities",
                        principalColumn: "university_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "departments",
                columns: table => new
                {
                    department_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    university_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    short_name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    head_of_department = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("departments_pkey", x => x.department_id);
                    table.ForeignKey(
                        name: "departments_university_id_fkey",
                        column: x => x.university_id,
                        principalTable: "universities",
                        principalColumn: "university_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "scholarships",
                columns: table => new
                {
                    scholarship_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    university_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    minimum_merit = table.Column<decimal>(type: "numeric(8,4)", precision: 8, scale: 4, nullable: true),
                    income_criteria = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    region_specific = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    coverage_percentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    coverage_amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    additional_benefits = table.Column<string>(type: "text", nullable: true),
                    application_deadline = table.Column<DateOnly>(type: "date", nullable: true),
                    academic_year = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("scholarships_pkey", x => x.scholarship_id);
                    table.ForeignKey(
                        name: "scholarships_university_id_fkey",
                        column: x => x.university_id,
                        principalTable: "universities",
                        principalColumn: "university_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    log_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    table_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    record_id = table.Column<Guid>(type: "uuid", nullable: true),
                    old_values = table.Column<string>(type: "jsonb", nullable: true),
                    new_values = table.Column<string>(type: "jsonb", nullable: true),
                    ip_address = table.Column<IPAddress>(type: "inet", nullable: true),
                    user_agent = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("audit_logs_pkey", x => x.log_id);
                    table.ForeignKey(
                        name: "audit_logs_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "educational_records",
                columns: table => new
                {
                    record_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    education_type = table.Column<EducationType>(type: "education_type", nullable: false),
                    institution_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    board_university = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    roll_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    total_marks = table.Column<int>(type: "integer", nullable: true),
                    obtained_marks = table.Column<int>(type: "integer", nullable: true),
                    percentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    grade = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    year_of_completion = table.Column<int>(type: "integer", nullable: true),
                    is_result_awaited = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("educational_records_pkey", x => x.record_id);
                    table.ForeignKey(
                        name: "educational_records_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mentors",
                columns: table => new
                {
                    mentor_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    designation = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    current_institution = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    graduation_year = table.Column<int>(type: "integer", nullable: true),
                    field_of_study = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    experience_years = table.Column<int>(type: "integer", nullable: true),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    linkedin_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    bio = table.Column<string>(type: "text", nullable: true),
                    profile_picture_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    specializations = table.Column<List<string>>(type: "text[]", nullable: true),
                    hourly_rate = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    availability_hours = table.Column<string>(type: "jsonb", nullable: true),
                    university_email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    university_id = table.Column<Guid>(type: "uuid", nullable: true),
                    verification_status = table.Column<VerificationStatus>(type: "verification_status", nullable: true),
                    total_sessions = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    average_rating = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: true, defaultValueSql: "0"),
                    is_active = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("mentors_pkey", x => x.mentor_id);
                    table.ForeignKey(
                        name: "mentors_university_id_fkey",
                        column: x => x.university_id,
                        principalTable: "universities",
                        principalColumn: "university_id");
                    table.ForeignKey(
                        name: "mentors_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "student_profiles",
                columns: table => new
                {
                    profile_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    gender = table.Column<GenderType>(type: "gender_type", nullable: true),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: true),
                    id_document_type = table.Column<IdDocumentType>(type: "id_document_type", nullable: true),
                    cnic = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    passport_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    nicop_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    guardian_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    father_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    guardian_relation = table.Column<GuardianRelation>(type: "guardian_relation", nullable: true),
                    guardian_phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    guardian_cnic = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    guardian_city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    guardian_address = table.Column<string>(type: "text", nullable: true),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    address = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    profile_picture_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    scholarship_priority = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    hostel_priority = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    guardian_income = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    preferred_admission_city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    household_income = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    is_hafiz_quran = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    is_orphan = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    disability = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    sports = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    profile_completed = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    completion_percentage = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)"),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("student_profiles_pkey", x => x.profile_id);
                    table.ForeignKey(
                        name: "student_profiles_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "test_scores",
                columns: table => new
                {
                    score_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    test_type = table.Column<TestType>(type: "test_type", nullable: false),
                    test_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    total_marks = table.Column<int>(type: "integer", nullable: true),
                    obtained_marks = table.Column<int>(type: "integer", nullable: true),
                    percentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    test_date = table.Column<DateOnly>(type: "date", nullable: true),
                    year = table.Column<int>(type: "integer", nullable: false),
                    roll_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("test_scores_pkey", x => x.score_id);
                    table.ForeignKey(
                        name: "test_scores_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "university_representatives",
                columns: table => new
                {
                    rep_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    university_id = table.Column<Guid>(type: "uuid", nullable: true),
                    designation = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    department = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    office_address = table.Column<string>(type: "text", nullable: true),
                    is_official = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    verification_status = table.Column<VerificationStatus>(type: "verification_status", nullable: true),
                    verification_document_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    permissions = table.Column<string>(type: "jsonb", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("university_representatives_pkey", x => x.rep_id);
                    table.ForeignKey(
                        name: "university_representatives_university_id_fkey",
                        column: x => x.university_id,
                        principalTable: "universities",
                        principalColumn: "university_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "university_representatives_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_logins",
                columns: table => new
                {
                    loginprovider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    providerkey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    providerdisplayname = table.Column<string>(type: "text", nullable: true),
                    userid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_logins", x => new { x.loginprovider, x.providerkey });
                    table.ForeignKey(
                        name: "fk_user_logins_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_refresh_tokens",
                columns: table => new
                {
                    token_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)"),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_revoked = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_refresh_tokens_pkey", x => x.token_id);
                    table.ForeignKey(
                        name: "user_refresh_tokens_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "programs",
                columns: table => new
                {
                    program_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    department_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    degree_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "'BS'::character varying"),
                    duration_years = table.Column<int>(type: "integer", nullable: true, defaultValue: 4),
                    total_credit_hours = table.Column<int>(type: "integer", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    eligibility_criteria = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("degree_programs_pkey", x => x.program_id);
                    table.ForeignKey(
                        name: "programs_department_id_fkey",
                        column: x => x.department_id,
                        principalTable: "departments",
                        principalColumn: "department_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mentorship_sessions",
                columns: table => new
                {
                    session_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    mentor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    student_id = table.Column<Guid>(type: "uuid", nullable: true),
                    session_type = table.Column<SessionType>(type: "session_type", nullable: false),
                    scheduled_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    duration_minutes = table.Column<int>(type: "integer", nullable: true, defaultValue: 60),
                    session_status = table.Column<SessionStatus>(type: "session_status", nullable: false),
                    actual_start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    actual_end_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    topic = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    session_notes = table.Column<string>(type: "text", nullable: true),
                    mentor_feedback = table.Column<string>(type: "text", nullable: true),
                    student_feedback = table.Column<string>(type: "text", nullable: true),
                    fee_amount = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    payment_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    payment_reference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    mentor_rating = table.Column<int>(type: "integer", nullable: true),
                    student_rating = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("mentorship_sessions_pkey", x => x.session_id);
                    table.ForeignKey(
                        name: "mentorship_sessions_mentor_id_fkey",
                        column: x => x.mentor_id,
                        principalTable: "mentors",
                        principalColumn: "mentor_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "mentorship_sessions_student_id_fkey",
                        column: x => x.student_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "applications",
                columns: table => new
                {
                    application_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    university_id = table.Column<Guid>(type: "uuid", nullable: true),
                    program_id = table.Column<Guid>(type: "uuid", nullable: true),
                    cycle_id = table.Column<Guid>(type: "uuid", nullable: true),
                    application_status = table.Column<ApplicationStatus>(type: "application_status", nullable: false),
                    application_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    submission_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    scholarship_applied = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    hostel_required = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    transport_required = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    calculated_merit = table.Column<decimal>(type: "numeric(8,4)", precision: 8, scale: 4, nullable: true),
                    merit_position = table.Column<int>(type: "integer", nullable: true),
                    fee_challan_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    fee_paid_amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    fee_payment_date = table.Column<DateOnly>(type: "date", nullable: true),
                    admission_offered = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    offer_date = table.Column<DateOnly>(type: "date", nullable: true),
                    offer_expires_at = table.Column<DateOnly>(type: "date", nullable: true),
                    rejection_reason = table.Column<string>(type: "text", nullable: true),
                    scheduled_submission_date = table.Column<DateOnly>(type: "date", nullable: true),
                    auto_submitted = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)"),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("applications_pkey", x => x.application_id);
                    table.ForeignKey(
                        name: "applications_cycle_id_fkey",
                        column: x => x.cycle_id,
                        principalTable: "admission_cycles",
                        principalColumn: "cycle_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "applications_program_id_fkey",
                        column: x => x.program_id,
                        principalTable: "programs",
                        principalColumn: "program_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "applications_university_id_fkey",
                        column: x => x.university_id,
                        principalTable: "universities",
                        principalColumn: "university_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "applications_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "merit_formulas",
                columns: table => new
                {
                    formula_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    university_id = table.Column<Guid>(type: "uuid", nullable: true),
                    program_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    test_weightage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    intermediate_weightage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    matric_weightage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    interview_weightage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true, defaultValueSql: "0"),
                    required_test_types = table.Column<List<TestType>>(type: "test_type[]", nullable: true),
                    minimum_test_score = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    minimum_intermediate_percentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    minimum_matric_percentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    academic_year = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("merit_formulas_pkey", x => x.formula_id);
                    table.ForeignKey(
                        name: "merit_formulas_program_id_fkey",
                        column: x => x.program_id,
                        principalTable: "programs",
                        principalColumn: "program_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "merit_formulas_university_id_fkey",
                        column: x => x.university_id,
                        principalTable: "universities",
                        principalColumn: "university_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "documents",
                columns: table => new
                {
                    document_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    application_id = table.Column<Guid>(type: "uuid", nullable: true),
                    educational_record_id = table.Column<Guid>(type: "uuid", nullable: true),
                    document_type = table.Column<DocumentType>(type: "document_type", nullable: false),
                    document_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    file_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    bucket = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    object_key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    storage_provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    checksum = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    file_size = table.Column<int>(type: "integer", nullable: true),
                    mime_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    metadata = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    verification_status = table.Column<VerificationStatus>(type: "verification_status", nullable: true),
                    verified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    verified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    rejection_reason = table.Column<string>(type: "text", nullable: true),
                    is_required = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    display_order = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("documents_pkey", x => x.document_id);
                    table.ForeignKey(
                        name: "documents_application_id_fkey",
                        column: x => x.application_id,
                        principalTable: "applications",
                        principalColumn: "application_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "documents_educational_record_id_fkey",
                        column: x => x.educational_record_id,
                        principalTable: "educational_records",
                        principalColumn: "record_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "documents_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "documents_verified_by_fkey",
                        column: x => x.verified_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    notification_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    read_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    related_application_id = table.Column<Guid>(type: "uuid", nullable: true),
                    related_session_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(now() AT TIME ZONE 'UTC'::text)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("notifications_pkey", x => x.notification_id);
                    table.ForeignKey(
                        name: "notifications_related_application_id_fkey",
                        column: x => x.related_application_id,
                        principalTable: "applications",
                        principalColumn: "application_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "notifications_related_session_id_fkey",
                        column: x => x.related_session_id,
                        principalTable: "mentorship_sessions",
                        principalColumn: "session_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "notifications_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_admission_cycles_university_id",
                table: "admission_cycles",
                column: "university_id");

            migrationBuilder.CreateIndex(
                name: "ix_applications_cycle_id",
                table: "applications",
                column: "cycle_id");

            migrationBuilder.CreateIndex(
                name: "ix_applications_program_id",
                table: "applications",
                column: "program_id");

            migrationBuilder.CreateIndex(
                name: "ix_applications_university_id_program_id",
                table: "applications",
                columns: new[] { "university_id", "program_id" });

            migrationBuilder.CreateIndex(
                name: "ix_applications_user_id",
                table: "applications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_user_id",
                table: "audit_logs",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_departments_university_id",
                table: "departments",
                column: "university_id");

            migrationBuilder.CreateIndex(
                name: "ix_documents_application_id",
                table: "documents",
                column: "application_id");

            migrationBuilder.CreateIndex(
                name: "ix_documents_educational_record_id",
                table: "documents",
                column: "educational_record_id");

            migrationBuilder.CreateIndex(
                name: "ix_documents_user_id",
                table: "documents",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_documents_verified_by",
                table: "documents",
                column: "verified_by");

            migrationBuilder.CreateIndex(
                name: "ix_educational_records_user_id",
                table: "educational_records",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_mentors_university_id",
                table: "mentors",
                column: "university_id");

            migrationBuilder.CreateIndex(
                name: "ix_mentors_user_id",
                table: "mentors",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_mentorship_sessions_mentor_id",
                table: "mentorship_sessions",
                column: "mentor_id");

            migrationBuilder.CreateIndex(
                name: "ix_mentorship_sessions_student_id",
                table: "mentorship_sessions",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "ix_merit_formulas_program_id",
                table: "merit_formulas",
                column: "program_id");

            migrationBuilder.CreateIndex(
                name: "ix_merit_formulas_university_id",
                table: "merit_formulas",
                column: "university_id");

            migrationBuilder.CreateIndex(
                name: "ix_notifications_related_application_id",
                table: "notifications",
                column: "related_application_id");

            migrationBuilder.CreateIndex(
                name: "ix_notifications_related_session_id",
                table: "notifications",
                column: "related_session_id");

            migrationBuilder.CreateIndex(
                name: "ix_notifications_user_id",
                table: "notifications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_programs_department_id",
                table: "programs",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_scholarships_university_id",
                table: "scholarships",
                column: "university_id");

            migrationBuilder.CreateIndex(
                name: "ix_student_profiles_user_id",
                table: "student_profiles",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_test_scores_user_id",
                table: "test_scores",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_university_representatives_university_id",
                table: "university_representatives",
                column: "university_id");

            migrationBuilder.CreateIndex(
                name: "ix_university_representatives_user_id",
                table: "university_representatives",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_logins_userid",
                table: "user_logins",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "ix_user_refresh_tokens_user_id",
                table: "user_refresh_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "ix_users_email1",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropTable(
                name: "documents");

            migrationBuilder.DropTable(
                name: "merit_formulas");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "scholarships");

            migrationBuilder.DropTable(
                name: "student_profiles");

            migrationBuilder.DropTable(
                name: "test_scores");

            migrationBuilder.DropTable(
                name: "university_representatives");

            migrationBuilder.DropTable(
                name: "user_logins");

            migrationBuilder.DropTable(
                name: "user_refresh_tokens");

            migrationBuilder.DropTable(
                name: "educational_records");

            migrationBuilder.DropTable(
                name: "applications");

            migrationBuilder.DropTable(
                name: "mentorship_sessions");

            migrationBuilder.DropTable(
                name: "admission_cycles");

            migrationBuilder.DropTable(
                name: "programs");

            migrationBuilder.DropTable(
                name: "mentors");

            migrationBuilder.DropTable(
                name: "departments");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "universities");
        }
    }
}
