using NpgsqlTypes;

namespace OneUniBackend.Enums;

public enum TestType
{
    [PgName("NET")]
    NET,
    [PgName("ECAT")]
    ECAT,
    [PgName("MDCAT")]
    MDCAT,
    [PgName("SAT")]
    SAT,
    [PgName("IELTS")]
    IELTS,
    [PgName("TOEFL")]
    TOEFL,
    [PgName("FAST")]
    FAST,
    [PgName("LUMS")]
    LUMS,
    [PgName("other")]
    Other
}
