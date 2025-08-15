using System.Text.Json;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Accounts.User.Dtos;

namespace game_x.application.Features.BankAccountVerifications.Commands._2_DecisionBankAccount;

public sealed class DecisionBankAccountHandler(
    IUnitOfWork unitOfWork,
    IUserBankAccountRepo bankAccountRepo,
    IFiatCurrencyRepo currencyRepo,
    INotificationRepo notificationRepo,
    IClientHubService clientHubService,
    IUserAccessor userAccessor) : ICommandHandler<DecisionBankAccountCommand>
{
    public async Task<Unit> Handle(DecisionBankAccountCommand request, CancellationToken ct = default)
    {
        var adminId = userAccessor.GetUserId();
        var userId = string.Empty;
        var currencyId = 0;

        await bankAccountRepo.UpdateAsync(request.Id, targetItem =>
        {
            if (targetItem.Status != UserBankAccountStatus.UnderReview)
                throw new BadRequestException(MessageCode.User.BankAccountInvalid);
            if (request.Status == UserBankAccountStatus.Approved)
                targetItem.Approve(adminId);
            else if (request.Status == UserBankAccountStatus.Rejected)
                targetItem.Reject(adminId, request.Reason!, request.Details!);
            else
                throw new BadRequestException(MessageCode.User.BankAccountStatusInvalid);
            userId = targetItem.UserId;
            currencyId = targetItem.CurrencyId;
        }, ct);
        await unitOfWork.SaveChangesAsync(ct);

        var targetCurrency = await currencyRepo.GetByIdAsync(currencyId, ct);

        var verificationDto = new VerificationStatusDto
        {
            CurrencyCode = targetCurrency?.Code?.Value ?? string.Empty,
            Type = VerificationStatusType.BankAccount,
            Status = (VerificationStatus)(int)request.Status,
            IsVerified = request.Status == UserBankAccountStatus.Approved,
        };

        await SendToMember(userId, verificationDto, ct);

        return Unit.Value;
    }

    private async Task SendToMember(string userId, VerificationStatusDto verificationDto, CancellationToken ct)
    {
        // Create notification for verification update
        var notification = Notification.Create(
            NotificationMessageKey.User_VerifyStatus_Changed,
            userId,
            NotificationType.Info,
            NotificationSeverity.Info,
            JsonSerializer.Serialize(verificationDto));
        await notificationRepo.AddNotificationAsync(notification, ct);

        await clientHubService.SendVerifyUpdateAsync(
            userId,
            verificationDto);
    }
}
