using NpgsqlTypes;

namespace OneUniBackend.Enums;

public enum GenderType
{
    [PgName("male")]
    Male,
    [PgName("female")]
    Female,
    [PgName("other")]
    Other,
    [PgName("prefer_not_to_say")]
    PreferNotToSay
}
