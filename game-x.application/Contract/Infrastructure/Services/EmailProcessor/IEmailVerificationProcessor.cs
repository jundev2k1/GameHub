namespace game_x.application.Contract.Infrastructure.Services.EmailProcessor;

public interface IEmailVerificationProcessor
{
    void SendVerificationEmail(string email);

    bool VerifyEmail(string email, string code);
}
