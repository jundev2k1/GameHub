namespace game_x.domain.Enum;

public enum NotificationMessageKey
{
    // Enum group key: Broadcast

    // Enum group key: System
    System_Error = 200,

    // Enum group key: background job
    Job_OrderCreated = 300,

    // Enum group key: Order module
    Order_Created = 400,
    Order_CreatedFail = 401,
    Order_Completed = 402,

    Order_Approved = 403
    // Enum group key: ...
}