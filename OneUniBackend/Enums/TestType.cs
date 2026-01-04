using NpgsqlTypes;

namespace OneUniBackend.Enums;

public enum TestType
{
    [PgName("net")]
    NET,
    [PgName("ecat")]
    ECAT,
    [PgName("mdcat")]
    MDCAT,
    [PgName("sat")]
    SAT,
    [PgName("ielts")]
    IELTS,
    [PgName("toefl")]
    TOEFL,
    [PgName("fast")]
    FAST,
    [PgName("lums")]
    LUMS,
    [PgName("other")]
    Other
}
