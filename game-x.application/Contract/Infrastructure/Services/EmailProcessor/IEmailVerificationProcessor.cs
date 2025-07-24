namespace game_x.application.Contract.Infrastructure.Services.EmailProcessor;

public interface IEmailVerificationProcessor
{
    Task SendVerificationEmailAsync(string email, CancellationToken ct = default);
}