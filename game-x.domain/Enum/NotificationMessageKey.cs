namespace game_x.domain.Enum;

public enum NotificationMessageKey : short
{
    // Enum group key: Broadcast

    // Enum group key: System
    System_Error = 200,

    // Enum group key: background job
    Job_OrderCreated = 300,

    // Enum group key: Transaction module
    Transaction_Created = 400,
    Transaction_Failed = 401,
    Transaction_Completed = 402,
    Transaction_Approved = 403,
    Transaction_Rejected = 404,
    Transaction_Reviewed = 405,

    // Enum group key: UserLedger module
    UserLedger_Created = 500,

    // Enum group key: Transaction module
    Balance_Updated = 600,

    // Enum group key: UserBankAccount - UserKyc module
    User_VerifyStatus_Changed = 700,
    User_Verify_Created = 701,

    // Enum group key: SocialLink module
    Friend_Request_Created = 800,
    Friend_Request_Accepted = 801,

    // Enum group key: Live Stream
    LiveStream_Upcoming = 900,
    LiveStream_TimeoutCancelled = 901,
    LiveStream_Ended = 902,
    LiveStream_Assigned = 903,
    LiveStream_ScheduleCancelled = 904,

    // Enum group key: ...
}
