using NpgsqlTypes;

namespace OneUniBackend.Enums;

public enum ApplicationStatus
{
    [PgName("draft")]
    Draft,
    [PgName("scheduled")]
    Scheduled,
    [PgName("submitted")]
    Submitted,
    [PgName("under_review")]
    UnderReview,
    [PgName("await_merit_list")]
    AwaitMeritList,
    [PgName("awaiting_fee_submission")]
    AwaitingFeeSubmission,
    [PgName("accepted")]
    Accepted,
    [PgName("rejected")]
    Rejected
}
