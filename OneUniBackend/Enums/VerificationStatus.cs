using NpgsqlTypes;

namespace OneUniBackend.Enums;

public enum VerificationStatus
{
    [PgName("pending")]
    Pending,
    [PgName("verified")]
    Verified,
    [PgName("rejected")]
    Rejected
}
