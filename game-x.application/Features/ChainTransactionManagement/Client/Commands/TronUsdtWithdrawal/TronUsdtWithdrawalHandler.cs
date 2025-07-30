using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.Uxm;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.ChainTransactionManagement.Mapping;
using game_x.application.Utils;
using game_x.share.ExternalApi.Uxm.Dtos;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.application.Features.ChainTransactionManagement.Client.Commands.TronUsdtWithdrawal;

public sealed class TronUsdtWithdrawalHandler(
    IUxmService uxmService,
    IUserRepo userRepo,
    IAuthService authService,
    IUnitOfWork unitOfWork,
    IAsymmetricCryptoService asymmetricCryptoService,
    IUserAccessor userAccessor,
    IChainTransactionRepo chainTransactionRepo,
    IApplicationEventDispatcher eventDispatcher,
    IOptions<GameXSettings> galaxySettings,
    IAsymmetricKeyCacheService asymmetricKeyCacheService) : ICommandHandler<TronUsdtWithdrawalCommand, Unit>
{
    public async Task<Unit> Handle(TronUsdtWithdrawalCommand request, CancellationToken ct)
    {
        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
           
            return Unit.Value;
        }
        catch (BadRequestException)
        {
            await unitOfWork.RollbackAsync(ct);
            throw new BadRequestException("Invalid signature.");
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw new BadRequestException("Order creation failed.");
        }
    }

    private async Task<ChainTransaction> CreateWithdrawalChainTransaction(TronUsdtWithdrawalCommand request, CancellationToken ct = default)
    {
        string userId = userAccessor.GetUserId();
        var ownerUser = await userRepo.GetUserByIdAsync(userId, ct);
        var role = await authService.GetRolesAsync(ownerUser);
        if (!role.IsUser) throw new ForbiddenException("Only user can create sell order.");
        
        var orderNumber = await OrderNoGenerator.GenerateUniqueOtcOrderNoAsync(chainTransactionRepo, ct);
        
        var order = ChainTransaction.Create(
            type: ChainTransactionType.Withdrawal,
            userId: userId,
            orderNumber: orderNumber,
            status: ChainTransactionStatus.Pending,
            amount: request.Amount,
            cryptoTokenId: 000,
            fromAddress: "",
            toAddress: "",
            transactionHash: "",
            note: "");
    
        // await orderRepo.AddAsync(order, ct);
        return order;
    }

    private Task<SecureRequest<UxmWithdrawalOrderRequest>> CreateUxmRequest(
        TronUsdtWithdrawalCommand request,
        Guid publicId)
    {
        var gameXPrivateKey = asymmetricKeyCacheService.GameXPrivateKey;
        var merchantNumber = galaxySettings.Value.MerchantNumber;
        
        // Create UXM request data
        var requestData = request.ToUxmWithdrawalOrderRequestData(merchantNumber);;
        var uxmRequest = new SecureRequest<UxmWithdrawalOrderRequest>
        {
            Data = requestData,
            Signature = asymmetricCryptoService.Sign(gameXPrivateKey, requestData)
        };
        return Task.FromResult(uxmRequest);
    }
}