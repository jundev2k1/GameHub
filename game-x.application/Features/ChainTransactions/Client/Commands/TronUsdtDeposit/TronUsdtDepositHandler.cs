using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.Uxm;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.ChainTransactions.Dtos;
using game_x.share.ExternalApi.Uxm.Dtos;
using Microsoft.Extensions.Configuration;

namespace game_x.application.Features.ChainTransactions.Client.Commands.TronUsdtDeposit;

public sealed class CreateDepositChainTransactionHandler(
    IUxmService uxmService,
    IChainTransactionRepo chainTransactionRepo,
     // IUserRepo userRepo,
     // IAuthService authService,
     IUnitOfWork unitOfWork,
    //IAsymmetricKeyRepo asymmetricKeyRepo,
    IAsymmetricCryptoService asymmetricCryptoService,
    IUserAccessor userAccessor,
    IConfiguration configuration,
    IUserBalanceRepo userBalanceRepo,
    IAsymmetricKeyCacheService asymmetricKeyCacheService
    //IApplicationEventDispatcher eventDispatcher
    ) : ICommandHandler<TronUsdtDepositCommand, CreateChainTransactionResponseDto>
{
    public async Task<CreateChainTransactionResponseDto> Handle(TronUsdtDepositCommand request, CancellationToken ct)

    {
        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            var userId = userAccessor.GetUserId();
            
            var localChainTransaction = await CreateLocalChainTransaction(request, ct);
            await unitOfWork.SaveChangesAsync(ct);
            // 1. Gọi API UXM
            var uxmRequest = await CreateUxmRequest(request, localChainTransaction.PublicId, ct);
            var apiResponse = await uxmService.CreateProxyChainTransactionDepositAsync(uxmRequest);
            // if (!apiResponse.IsSuccessStatusCode || apiResponse.Content == null)
            //     throw new BadRequestException("UXM API failed.");
            var result = apiResponse;

            // cập nhật status khi gọi dc uxm
            await chainTransactionRepo.UpdateAsync(
                localChainTransaction.PublicId,
                tx =>
                {
                    tx.Status = ChainTransactionStatus.Approved;
                    tx.UpdatedAt = DateTime.UtcNow;
                },
                ct
            );

            var uxmPublicKey = asymmetricKeyCacheService.UxmPublicKey;
            // 2. Xác minh chữ ký từ UXM
            // var uxmPublicKey = await asymmetricKeyRepo
            //     .GetByCompositeKeyAsync(AsymmetricKeyNames.Uxm, AsymmetricKeyType.Public, AsymmetricType.ECDSA, ct);

            var isValid = asymmetricCryptoService
                // .VerifySignature(uxmPublicKey.KeyValue, result.Data, result.Signature);
                .VerifySignature(uxmPublicKey, result.Data, result.Signature);

            if (!isValid)
                throw new BadRequestException("Invalid signature from UXM.");



            await UpdateUserBalanceAsync(userId, (int)CryptoType.Trc20Usdt, request.Amount, ct);

            // 6. Commit local Order to Galaxy Pay DB
            await unitOfWork.CommitAsync(ct);
            // 3. Trả về kết quả (ví dụ trả thẳng data)
            return new CreateChainTransactionResponseDto
            {
                OrderUid = result.Data.OrderUid,
                Amount = result.Data.Amount,
                To = result.Data.To
            };

        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }

    }
    private async Task<ChainTransaction> CreateLocalChainTransaction(TronUsdtDepositCommand request, CancellationToken ct = default)
    {
        // var ownerUser = await userRepo.GetUserByIdAsync(request.MemberId, ct);
        //var role = await authService.GetRolesAsync(ownerUser);
        //if (!role.IsUser) throw new ForbiddenException("Only user can create sell order.");

        var userId = userAccessor.GetUserId();


        var transaction = ChainTransaction.Create(
            userId: userId,
        orderNumber: "string",
        cryptoTokenId: 1,
        amount: request.Amount,
        type: ChainTransactionType.Deposit,
        status: ChainTransactionStatus.Pending
        );

        await chainTransactionRepo.AddAsync(transaction, ct);
        return transaction;
    }
    private async Task<SecureRequest<CreateChainTransactionDepositRequestData>> CreateUxmRequest(
        TronUsdtDepositCommand request,
        Guid publicId,
        CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        // Line ~50: Get Galaxy Pay private key

        var privateKeyPem = asymmetricKeyCacheService.GameXPrivateKey;


        // var privateKeyPem = await asymmetricKeyRepo
        //     .GetByCompositeKeyAsync(AsymmetricKeyNames.GameX, AsymmetricKeyType.Private, AsymmetricType.ECDSA, ct);


        // Get merchant number from configuration
        var merchantNumber = configuration.GetValue<string>("GameXSettings:MerchantNumber")
            ?? throw new Exception("MerchantNumber is not yet configured.");

        // Line ~58: Create request data
        var requestData = new CreateChainTransactionDepositRequestData(
            merchantNumber,
            request.Amount,
            publicId.ToString(),
            userId,
            request.Note
        );

        // Line ~64: Generate signature
        //  var signature = asymmetricCryptoService.Sign(privateKeyPem.KeyValue, requestData);
        var signature = asymmetricCryptoService.Sign(privateKeyPem, requestData);

        // Line ~66: Return secure request
        return new SecureRequest<CreateChainTransactionDepositRequestData>
        {
            Data = requestData,
            Signature = signature
        };
    }

    private async Task UpdateUserBalanceAsync(string userId, int cryptoTokenId, decimal amount, CancellationToken ct)
    {
        var balance = await userBalanceRepo
            .GetByUserIdAndTokenIdAsync(userId, cryptoTokenId, ct);

        if (balance == null)
            throw new NotFoundException("UserBalance not found.");

        balance.Amount += amount;
        balance.UpdatedAt = DateTime.UtcNow;
    }


}
