using NpgsqlTypes;

namespace OneUniBackend.Enums;

public enum UserRole

{
    [PgName("student")]
    student = 0,
    [PgName("mentor")]
    mentor = 1,
    [PgName("university_representative")]
    university_representative = 2,
    [PgName("admin")]
    admin = 3

}
