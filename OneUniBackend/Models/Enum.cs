using System;
namespace OneUniBackend.Models;

public enum UserRole
{
    Student,
    Mentor,
    UniversityRepresentative,
    Admin
}

public enum GenderType
{
    Male,
    Female,
    Other,
    PreferNotToSay
}

public enum EducationType
{
    Matric,
    Intermediate,
    ALevels,
    OLevels,
    Equivalent
}

public enum TestType
{
    NET,
    ECAT,
    MDCAT,
    SAT,
    IELTS,
    TOEFL,
    FAST,
    LUMS,
    Other
}

public enum ApplicationStatus
{
    Draft,
    Scheduled,
    Submitted,
    UnderReview,
    AwaitMeritList,
    AwaitingFeeSubmission,
    Accepted,
    Rejected
}

public enum SessionType
{
    Free,
    Paid
}

public enum SessionStatus
{
    Scheduled,
    Completed,
    Cancelled,
    NoShow
}

public enum DocumentType
{
    MatricCertificate,
    IntermediateCertificate,
    Transcript,
    Cnic,
    Passport,
    Nicop,
    BForm,
    SportsCertificate,
    HafizCertificate,
    IncomeCertificate,
    Domicile,
    Other
}

public enum VerificationStatus
{
    Pending,
    Verified,
    Rejected
}

public enum GuardianRelation
{
    Father,
    Mother,
    Guardian,
    Other
}

public enum IdDocumentType
{
    Cnic,
    Nicop,
    Passport,
    BForm
}