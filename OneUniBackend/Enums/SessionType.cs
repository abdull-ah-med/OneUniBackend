using NpgsqlTypes;

namespace OneUniBackend.Enums;

public enum SessionType
{
    [PgName("free")]
    Free,
    [PgName("paid")]
    Paid
}
