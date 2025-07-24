using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.Uxm;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnOrderCreated;
using game_x.application.Features.OrderManagement.Dtos;
using game_x.application.Mappers.Order;
using game_x.share.ExternalApi.Uxm.Dtos;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.application.Features.OrderManagement.Staff.Commands.TraceV2.Sell;

public sealed class CreateSellOrderV2Handler(
    OrderMapper orderMapper,
    IUxmService uxmService,
    IUserRepo userRepo,
    IAuthService authService,
    IOrderRepo orderRepo,
    IUnitOfWork unitOfWork,
    IAsymmetricCryptoService asymmetricCryptoService,
    IStaffCounterRepo staffCounterRepo,
    IUserAccessor userAccessor,
    IOptions<GalaxySettings> galaxySettings,
    IAsymmetricKeyCacheService  asymmetricKeyCacheService,
    IApplicationEventDispatcher eventDispatcher)
    : ICommandHandler<CreateSellOrderV2Command, CreateOrderV2ResponseDto>
{
    public async Task<CreateOrderV2ResponseDto> Handle(CreateSellOrderV2Command request, CancellationToken ct = default)
    {
        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            // 1. Create a local Order at Galaxy Pay
            var localOrder = await CreateLocalOrder(request, ct);

            // 2. Call to UXM server to create Order
            var uxmRequest = await CreateUxmRequest(request, localOrder.PublicId);
            var result = await uxmService.CreateProxySellOrderV2Async(uxmRequest);

            // 3. Verify UXM signature
            var uxmPublicKey = asymmetricKeyCacheService.UxmPublicKey;
            var isValid = asymmetricCryptoService.VerifySignature(publicKeyPem: uxmPublicKey, data: result.Data, signatureBase64: result.Signature);
            if (!isValid) throw new BadRequestException();

            // 4. Update the order based on the response from the Uxm order.
            localOrder.UpdateUxmOrder(
                orderUid: result.Data.OrderUid,
                entryCode: result.Data.EntryCode,
                fiatAmount: result.Data.FiatAmount,
                cryptoAmount: result.Data.CryptoAmount,
                fee: result.Data.Fee,
                timestamp: result.Data.Timestamp);

            // 5. Commit local Order to Galaxy Pay DB
            await unitOfWork.CommitAsync(ct);

            // 6. Notice the member for updating order's status
            await eventDispatcher.Publish(new OnOrderCreatedEvent(localOrder), ct);

            return orderMapper.ToCreateOrderV2Dto(result.Data, isValid);
        }
        catch (BadRequestException)
        {
            await unitOfWork.RollbackAsync(ct);
            throw new BadRequestException("Invalid signature.");
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }

    private async Task<Order> CreateLocalOrder(CreateSellOrderV2Command request, CancellationToken ct = default)
    {
        var ownerUser = await userRepo.GetUserByIdAsync(request.MemberId, ct);
        var role = await authService.GetRolesAsync(ownerUser);
        if (!role.IsUser) throw new ForbiddenException("Only user can create sell order.");
        
        var staffId = userAccessor.GetUserId();
        var counter = await staffCounterRepo.GetTrackingLogAsync(staffId, ct);
        
        var order = Order.Create(
            type: OrderType.Sell,
            userId: request.MemberId,
            counterId: counter.CounterId,
            staffId: staffId,
            pricingMode: request.PricingMode,
            fiatType: request.FiatType,
            cryptoType: request.CryptoType,
            quantity: 1,
            status: OrderStatus.Created,
            payeeBranchCode: request.PayeeBranchCode,
            payeeAccountNumber: request.PayeeAccountNumber,
            payeeBankName: request.PayeeBankName,
            payeeName: request.PayeeName);
        
        await orderRepo.AddAsync(order, ct);
        return order;
    }

    private Task<SecureRequest<CreateSellOrderV2Request>> CreateUxmRequest(
        CreateSellOrderV2Command request,
        Guid publicId)
    {
        var galaxyPrivateKey = asymmetricKeyCacheService.GalaxyPrivateKey;
        var merchantNumber = galaxySettings.Value.MerchantNumber;
        
        // Create UXM request data
        var requestData = orderMapper.ToCreateSellOrderV2RequestData(
            request,
            publicId.ToString(),
            merchantNumber);

        var uxmRequest = new SecureRequest<CreateSellOrderV2Request>
        {
            Data = requestData,
            Signature = asymmetricCryptoService.Sign(galaxyPrivateKey, requestData)
        };
        
        return Task.FromResult(uxmRequest);
    }
}
