using game_x.application.Contract.Infrastructure.Email;
using game_x.application.Exceptions;
using game_x.share.Extensions;
using game_x.share.Settings;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.RegularExpressions;

namespace game_x.infrastructure.Email;

public class EngageLabEmailService(
    IOptions<EngageLabSettings> options,
    IEngageLabEmailApi emailApi) : IEmailService
{
    public async Task SendAsync(string to, string subject, string htmlBody)
    {
        var from = options.Value.From;
        var contentText = StripHtmlTags(htmlBody);
        var previewText = contentText.Length > 80 ? contentText[..80] + "..." : contentText;

        var request = new EngageLabEmailRequest
        {
            From = from,
            To = [to],
            Body = new EngageLabEmailBody
            {
                Subject = subject,
                Content = new EngageLabEmailContent
                {
                    Html = htmlBody,
                    Text = contentText,
                    PreviewText = previewText
                },
                Settings = new EngageLabEmailSettings
                {
                    SendMode = 0
                }
            }
        };

        var response = await emailApi.SendEmailAsync(request);
        if (!response.IsSuccessStatusCode || response.Content?.EmailIds == null || !response.Content.EmailIds.Any())
        {
            var apiError = response.Error?.Content ?? response.ReasonPhrase ?? "unknown error";
            throw new ExternalServiceException($"EngageLab error：{apiError}");
        }
    }

    public Task SendResetPasswordEmailAsync(string to, string code)
    {
        var subject = "Reset Your Password Verification Code";
        var bodyBuilder = new StringBuilder();
        bodyBuilder.AppendLine("<p>Please enter the following code to reset your password:</p>");
        bodyBuilder.AppendLine($"    <h2>{code}</h2>");
        bodyBuilder.AppendLine("<p>This code will expire in 10 minutes.</p>");

        return SendAsync(to, subject, bodyBuilder.ToString());
    }

    public Task SendVerificationEmailAsync(string to, string code)
    {
        var subject = "Email Verification Code";
        var bodyBuilder = new StringBuilder();
        bodyBuilder.AppendLine("<p>Please use the following code to verify your email address:</p>");
        bodyBuilder.AppendLine($"   <h2>{code}</h2>");
        bodyBuilder.AppendLine("<p>This code will expire in 10 minutes.</p>");
        return SendAsync(to, subject, bodyBuilder.ToString());
    }

    public static string StripHtmlTags(string html)
    {
        return !html.IsNullOrWhiteSpace()
            ? Regex.Replace(html, "<.*?>", string.Empty)
            : string.Empty;
    }
}
