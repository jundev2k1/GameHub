namespace game_x.share.ExternalApi.FastPay.Dtos;

public record FastPayErrorResponse(
    string? Type,
    string? Title,
    int Status,
    object? Errors,
    int ErrorCode);
