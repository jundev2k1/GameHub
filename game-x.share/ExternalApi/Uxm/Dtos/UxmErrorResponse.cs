namespace game_x.share.ExternalApi.Uxm.Dtos;

public record UxmErrorResponse(
    string? Type,
    string? Title,
    int Status,
    object? Errors,
    int ErrorCode);
