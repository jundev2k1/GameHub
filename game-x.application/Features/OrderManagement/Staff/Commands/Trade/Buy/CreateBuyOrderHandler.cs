using game_x.application.Contract.Infrastructure.ExternalApi.Uxm;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnOrderCreated;
using game_x.application.Features.OrderManagement.Dtos;
using game_x.application.Mappers.Order;
using game_x.share.ExternalApi.Uxm.Dtos;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace game_x.application.Features.OrderManagement.Staff.Commands.Trade.Buy;

public sealed class CreateBuyOrderHandler(
    OrderMapper orderMapper,
    IUxmService uxmService,
    IOrderRepo orderRepo,
    IUserRepo userRepo,
    IAuthService authService,
    IUnitOfWork unitOfWork,
    IAsymmetricKeyRepo asymmetricKeyRepo,
    IAsymmetricCryptoService asymmetricCryptoService,
    IStaffCounterRepo staffCounterRepo,
    IUserAccessor userAccessor,
    IConfiguration configuration,
    IApplicationEventDispatcher eventDispatcher) : ICommandHandler<CreateBuyOrderCommand, CreateOrderResponseDto>
{
    public async Task<CreateOrderResponseDto> Handle(CreateBuyOrderCommand request, CancellationToken ct)
    {
        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            // 1. Create a local Order at Galaxy Pay
            var localOrder = await CreateLocalOrder(request, ct);

            // 2. Call to UXM server to create Order
            var uxmRequest = await CreateUxmRequest(request, localOrder.PublicId, ct);
            var result = await uxmService.CreateProxyBuyOrderAsync(uxmRequest);

            // 3. Verify UXM signature
            var uxmPublicKey = await asymmetricKeyRepo
                .GetByCompositeKeyAsync(AsymmetricKeyNames.Uxm, KeyType.Public, AsymmetricType.ECDSA, ct);
            var isValid = asymmetricCryptoService
                .VerifySignature(uxmPublicKey.KeyValue, result.Data, result.Signature);
            if (!isValid) throw new BadRequestException("Invalid signature.");

            // 4. Update Order Uid after UXM returns response
            localOrder.UpdateOrderUid(result.Data.OrderUid);

            // 5. Update Order Entry Code
            var newMetadata = new Dictionary<string, object>
            {
                { "entryCode", result.Data.EntryCode }
            };
            localOrder.UpdateMetadata(JsonSerializer.Serialize(newMetadata));

            // 6. Commit local Order to Galaxy Pay DB
            await unitOfWork.CommitAsync(ct);

            // 7. Notice the member for updating order's status
            await eventDispatcher.Publish(new OnOrderCreatedEvent(localOrder!), ct);

            return orderMapper.ToCreateOrderDto(result.Data, isValid);
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }

    private async Task<Order> CreateLocalOrder(CreateBuyOrderCommand request, CancellationToken ct = default)
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
            quantity: 1,
            pricePerUnit: request.FiatAmount,
            currencyUnit: CurrencyUnit.Of(request.FiatType ?? CurrencyCode.CNY),
            status: OrderStatus.Created);
        await orderRepo.AddAsync(order, ct);
        return order;
    }

    private async Task<SecureRequest<CreateOrderBuyRequestData>> CreateUxmRequest(
        CreateBuyOrderCommand request,
        Guid publicId,
        CancellationToken ct = default)
    {
        // Get Galaxy Pay private key
        var privateKeyPem = await asymmetricKeyRepo
            .GetByCompositeKeyAsync(AsymmetricKeyNames.GalaxyPay, KeyType.Private, AsymmetricType.ECDSA, ct);

        // Get merchant number from configuration
        var merchantNumber = configuration.GetValue<string>("GalaxySettings:MerchantNumber")
            ?? throw new Exception("MerchantNumber is not yet configured.");

        // Create UXM request data
        var requestData = orderMapper.ToCreateOrderBuyRequestData(request, publicId.ToString(), merchantNumber);
        var uxmRequest = new SecureRequest<CreateOrderBuyRequestData>
        {
            Data = requestData,
            Signature = asymmetricCryptoService.Sign(privateKeyPem.KeyValue, requestData)
        };
        return uxmRequest;
    }
}
