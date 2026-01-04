using NpgsqlTypes;

namespace OneUniBackend.Enums;

public enum EducationType
{
    [PgName("matric")]
    Matric,
    [PgName("intermediate")]
    Intermediate,
    [PgName("a_levels")]
    ALevels,
    [PgName("o_levels")]
    OLevels,
    [PgName("equivalent")]
    Equivalent
}
