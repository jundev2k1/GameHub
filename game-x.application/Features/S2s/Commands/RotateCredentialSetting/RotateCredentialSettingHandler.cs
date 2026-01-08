using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.S2s.Commands.RotateCredentialSetting;

public sealed class RotateCredentialSettingHandler(
    IUnitOfWork unitOfWork,
    IS2sCredentialRepo s2SCredentialRepo,
    IAuthGenerator authGenerator) : ICommandHandler<RotateCredentialSettingCommand>
{
    public async Task<Unit> Handle(RotateCredentialSettingCommand request, CancellationToken ct = default)
    {
        var currentCredential = await s2SCredentialRepo.GetByKeyIdAsync(
            request.KeyId,
            CredentialDirection.Inbound,
            ct);
        if (currentCredential.ClientSetting.AppCode != request.AppCode
            || currentCredential.ClientSetting.ClientId != request.ClientId)
            throw new BadRequestException($"AppCode({request.AppCode}) or ClientId({request.ClientId}) are invalid.");

        var credential = S2SCredential.Create(
            currentCredential.AuthMethod,
            CredentialDirection.Inbound,
            KeyUsageScope.ApiRequest,
            currentCredential.SettingId);
        var materialItems = GetMaterialItems(currentCredential.AuthMethod);
        credential.AddMaterials(materialItems);

        await unitOfWork.WithTransactionAsync(async () =>
        {
            await s2SCredentialRepo.RotateAsync(request.KeyId, credential, ct);
        }, ct);

        return Unit.Value;
    }

    private IEnumerable<S2SCredentialMaterial> GetMaterialItems(AuthMethod method)
    {
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
