using NpgsqlTypes;

namespace OneUniBackend.Enums;

public enum GuardianRelation
{
    [PgName("father")]
    Father,
    [PgName("mother")]
    Mother,
    [PgName("guardian")]
    Guardian,
    [PgName("other")]
    Other
}
