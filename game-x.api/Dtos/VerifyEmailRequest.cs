using game_x.api.Enums;

namespace game_x.api.Dtos;

public record VerifyEmailRequest(string Email, string Code, EmailVerificationPurpose Purpose);
