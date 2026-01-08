using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.S2s.Commands.CreateCredentitalSetting;

public sealed class CreateCredentitalSettingHandler(
    IUnitOfWork unitOfWork,
    IS2sClientSettingRepo s2SClientSettingRepo,
    IS2sCredentialRepo s2SCredentialRepo,
    IAuthGenerator authGenerator) : ICommandHandler<CreateCredentitalSettingCommand>
{
    public async Task<Unit> Handle(CreateCredentitalSettingCommand request, CancellationToken ct = default)
    {
        var isExist = await s2SCredentialRepo.CanAddKeyAsync(request.AppCode, CredentialDirection.Inbound, ct);
        if (isExist)
            throw new BadRequestException(MessageCode.System.Conflict);

        var s2sSetting = await s2SClientSettingRepo.GetByAppCodeAsync(request.AppCode, ct);
        var credential = S2SCredential.Create(
            request.Method,
            CredentialDirection.Inbound,
            KeyUsageScope.ApiRequest,
            s2sSetting.Id);
        var materialItems = GetMaterialItems(request.Method, request.Keys);
        credential.AddMaterials(materialItems);

        await unitOfWork.WithTransactionAsync(async () =>
        {
            await s2SCredentialRepo.CreateAsync(credential, ct);
        }, ct);

        return Unit.Value;
    }

    private IEnumerable<S2SCredentialMaterial> GetMaterialItems(
        AuthMethod method,
        CredentialMaterialItem[] items)
    {
        if (items.Length > 0)
        {
            foreach (var item in items)
            {
                yield return S2SCredentialMaterial.Create(item.Type, item.Value, item.IsEncrypted);
            }

            yield break;
        }

        switch (method)
        {
            case AuthMethod.ApiKey:
                var apiKey = authGenerator.GenerateApiKey();
                yield return S2SCredentialMaterial.Create(CredentialMaterialType.ApiKey, apiKey, false);
                break;

            case AuthMethod.Hmac:
                var hmacSerret = authGenerator.GenerateHmacKey();
                yield return S2SCredentialMaterial.Create(CredentialMaterialType.HmacSecret, hmacSerret, false);
                break;

            case AuthMethod.Rsa:
                var (rsaPublic, rsaPrivate) = authGenerator.GenerateRsaKeys();
                yield return S2SCredentialMaterial.Create(CredentialMaterialType.RsaPrivateKey, rsaPrivate, false);
                yield return S2SCredentialMaterial.Create(CredentialMaterialType.RsaPublicKey, rsaPublic, false);
                break;

            case AuthMethod.Ecdsa:
                var (ecdsaPublic, ecdsaPrivate) = authGenerator.GenerateEcdsaKeys();
                yield return S2SCredentialMaterial.Create(CredentialMaterialType.EcdsaPrivateKey, ecdsaPrivate, false);
                yield return S2SCredentialMaterial.Create(CredentialMaterialType.EcdsaPublicKey, ecdsaPublic, false);
                break;

            default:
                throw new NotSupportedException();
        }
    }
}
