using NpgsqlTypes;

namespace OneUniBackend.Enums;

public enum SessionStatus
{
    [PgName("scheduled")]
    Scheduled,
    [PgName("completed")]
    Completed,
    [PgName("cancelled")]
    Cancelled,
    [PgName("no_show")]
    NoShow
}
