using Npgsql;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace OneUniBackend.Extensions;

public static class EnumMappings
{
    public static void Map(NpgsqlDataSourceBuilder builder)
    {
        builder.MapEnum<OneUniBackend.Enums.UserRole>("user_role");
        builder.MapEnum<OneUniBackend.Enums.GenderType>("gender_type");
        builder.MapEnum<OneUniBackend.Enums.EducationType>("education_type");
        builder.MapEnum<OneUniBackend.Enums.TestType>("test_type");
        builder.MapEnum<OneUniBackend.Enums.ApplicationStatus>("application_status");
        builder.MapEnum<OneUniBackend.Enums.SessionType>("session_type");
        builder.MapEnum<OneUniBackend.Enums.SessionStatus>("session_status");
        builder.MapEnum<OneUniBackend.Enums.DocumentType>("document_type");
        builder.MapEnum<OneUniBackend.Enums.VerificationStatus>("verification_status");
        builder.MapEnum<OneUniBackend.Enums.GuardianRelation>("guardian_relation");
        builder.MapEnum<OneUniBackend.Enums.IdDocumentType>("id_document_type");
    }

     public static void MapEnumsforEF(this NpgsqlDbContextOptionsBuilder builder)
    {
        builder.MapEnum<OneUniBackend.Enums.UserRole>("user_role");
        builder.MapEnum<OneUniBackend.Enums.GenderType>("gender_type");
        builder.MapEnum<OneUniBackend.Enums.EducationType>("education_type");
        builder.MapEnum<OneUniBackend.Enums.TestType>("test_type");
        builder.MapEnum<OneUniBackend.Enums.ApplicationStatus>("application_status");
        builder.MapEnum<OneUniBackend.Enums.SessionType>("session_type");
        builder.MapEnum<OneUniBackend.Enums.SessionStatus>("session_status");
        builder.MapEnum<OneUniBackend.Enums.DocumentType>("document_type");
        builder.MapEnum<OneUniBackend.Enums.VerificationStatus>("verification_status");
        builder.MapEnum<OneUniBackend.Enums.GuardianRelation>("guardian_relation");
        builder.MapEnum<OneUniBackend.Enums.IdDocumentType>("id_document_type");
    }
}
