using game_x.api.Enums;

namespace game_x.api.Dtos;

public record ResendCodeRequest(string? Email, EmailVerificationPurpose Purpose);
