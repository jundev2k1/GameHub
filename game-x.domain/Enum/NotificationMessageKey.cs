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
    Verify_Updated = 700,

    // Enum group key: ...
}
