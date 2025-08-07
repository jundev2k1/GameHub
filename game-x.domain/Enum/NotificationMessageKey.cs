namespace game_x.domain.Enum;

public enum NotificationMessageKey : short
{
    // Enum group key: Broadcast

    // Enum group key: System
    System_Error = 200,

    // Enum group key: background job
    Job_OrderCreated = 300,

    // Enum group key: Order module
    Transaction_Created = 400,
    Transaction_Failed = 401,
    Transaction_Completed = 402,
    Transaction_Approved = 403,
    Transaction_Rejected = 404,
    Transaction_Reviewed = 405,
    // Enum group key: ...
}