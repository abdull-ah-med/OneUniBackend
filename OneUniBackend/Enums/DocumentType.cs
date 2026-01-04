using NpgsqlTypes;

namespace OneUniBackend.Enums;

public enum DocumentType
{
    [PgName("matric_certificate")]
    MatricCertificate,
    [PgName("intermediate_certificate")]
    IntermediateCertificate,
    [PgName("transcript")]
    Transcript,
    [PgName("cnic")]
    Cnic,
    [PgName("passport")]
    Passport,
    [PgName("nicop")]
    Nicop,
    [PgName("b_form")]
    BForm,
    [PgName("sports_certificate")]
    SportsCertificate,
    [PgName("hafiz_certificate")]
    HafizCertificate,
    [PgName("income_certificate")]
    IncomeCertificate,
    [PgName("domicile")]
    Domicile,
    [PgName("other")]
    Other
}
