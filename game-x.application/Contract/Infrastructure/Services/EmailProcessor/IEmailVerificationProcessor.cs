namespace game_x.application.Contract.Infrastructure.Services.EmailProcessor;

public interface IEmailVerificationProcessor
{
    void SendVerificationEmail(string email, string purpose);

    bool VerifyEmail(string email, string code, string purpose);
}
