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

namespace game_x.application.Features.OrderManagement.Staff.Commands.TraceV2.Buy;

public sealed class CreateBuyOrderV2Handler(
    OrderMapper orderMapper,
    IUxmService uxmService,
    IOrderRepo orderRepo,
    IUserRepo userRepo,
    IAuthService authService,
    IUnitOfWork unitOfWork,
    IAsymmetricCryptoService asymmetricCryptoService,
    IStaffCounterRepo staffCounterRepo,
    IUserAccessor userAccessor,
    IApplicationEventDispatcher eventDispatcher,
    IOptions<GalaxySettings> galaxySettings,
    IAsymmetricKeyCacheService  asymmetricKeyCacheService) : ICommandHandler<CreateBuyOrderV2Command, CreateOrderV2ResponseDto>
{
    public async Task<CreateOrderV2ResponseDto> Handle(CreateBuyOrderV2Command request, CancellationToken ct)
    {
        await unitOfWork.BeginTransactionAsync(ct);

        try
        {
            // 1. Create a local Order at Galaxy Pay
            var localOrder = await CreateLocalOrder(request, ct);

            // 2. Call to UXM server to create Order
            var uxmRequest = await CreateUxmRequest(request, localOrder.PublicId);
            var result = await uxmService.CreateProxyBuyOrderV2Async(uxmRequest);

            // 3. Verify UXM signature
            var uxmPublicKey = asymmetricKeyCacheService.UxmPublicKey;
            var isValid =
                asymmetricCryptoService.VerifySignature(uxmPublicKey, result.Data, result.Signature);
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
            await eventDispatcher.Publish(new OnOrderCreatedEvent(localOrder!), ct);

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
            throw new BadRequestException("Order creation failed.");
        }
    }

    private async Task<Order> CreateLocalOrder(CreateBuyOrderV2Command request, CancellationToken ct = default)
    {
        var ownerUser = await userRepo.GetUserByIdAsync(request.MemberId, ct);
        var role = await authService.GetRolesAsync(ownerUser);
        if (!role.IsUser) throw new ForbiddenException("Only user can create sell order.");
        
        var staffId = userAccessor.GetUserId();
        var counter = await staffCounterRepo.GetTrackingLogAsync(staffId, ct);

        var order = Order.Create(
            type: OrderType.Buy,
            userId: request.MemberId,
            counterId: counter.CounterId,
            staffId: staffId,
            pricingMode: request.PricingMode,
            fiatType: request.FiatType,
            cryptoType: request.CryptoType,
            quantity: 1,
            status: OrderStatus.Created);

        await orderRepo.AddAsync(order, ct);
        return order;
    }

    private Task<CreateBuyOrderV2Request> CreateUxmRequest(
        CreateBuyOrderV2Command request,
        Guid publicId)
    {
        var galaxyPrivateKey = asymmetricKeyCacheService.GalaxyPrivateKey;
        var merchantNumber = galaxySettings.Value.MerchantNumber;
        
        // Create UXM request data
        var requestData = orderMapper.ToCreateOrderV2ReqData(request);
        requestData.MerchantOrderId = publicId.ToString();
        requestData.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        requestData.MerchantNumber = merchantNumber;
        var uxmRequest = new CreateBuyOrderV2Request
        {
            Data = requestData,
            Signature = asymmetricCryptoService.Sign(galaxyPrivateKey, requestData)
        };
        return Task.FromResult(uxmRequest);
    }
}
