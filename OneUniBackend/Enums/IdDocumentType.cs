using NpgsqlTypes;

namespace OneUniBackend.Enums;

public enum IdDocumentType
{
    [PgName("cnic")]
    Cnic,
    [PgName("nicop")]
    Nicop,
    [PgName("passport")]
    Passport,
    [PgName("b_form")]
    BForm
}
