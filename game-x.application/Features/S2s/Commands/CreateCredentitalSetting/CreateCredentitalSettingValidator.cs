namespace game_x.application.Features.S2s.Commands.CreateCredentitalSetting;

public sealed class CreateCredentitalSettingValidator : AbstractValidator<CreateCredentitalSettingCommand>
{
    public CreateCredentitalSettingValidator()
    {
        RuleFor(x => x.Keys)
            .IsInEnum().WithMessage("Authentication method is invalid.");

        RuleFor(x => x.Keys)
            .NotNull().WithMessage("Credential keys are required.");

        RuleFor(x => x)
            .Custom(ValidateByMethod);
    }

    private static void ValidateByMethod(
        CreateCredentitalSettingCommand command,
        ValidationContext<CreateCredentitalSettingCommand> context)
    {
        switch (command.Method)
        {
            case AuthMethod.ApiKey:
                ValidateApiKey(command, context);
                break;

            case AuthMethod.Hmac:
                ValidateHmac(command, context);
                break;

            case AuthMethod.Rsa or AuthMethod.Ecdsa:
                ValidateRsa(command, context);
                break;

            default:
                context.AddFailure("Method", "Unsupported authentication method.");
                break;
        }
    }

    private static void ValidateApiKey(
        CreateCredentitalSettingCommand command,
        ValidationContext<CreateCredentitalSettingCommand> context)
    {
        if (command.Keys.Length == 0)
            return;

        if (command.Keys.Length != 1)
        {
            context.AddFailure("Keys", "API Key authentication requires exactly 1 key.");
            return;
        }

        if (command.Keys[0].Type != CredentialMaterialType.ApiKey)
        {
            context.AddFailure("Keys[0].Type", "Invalid key type for API Key authentication.");
        }
    }

    private static void ValidateHmac(
        CreateCredentitalSettingCommand command,
        ValidationContext<CreateCredentitalSettingCommand> context)
    {
        if (command.Keys.Length == 0)
            return;

        if (command.Keys.Length != 1)
        {
            context.AddFailure("Keys", "HMAC authentication requires exactly 1 secret.");
            return;
        }

        if (command.Keys[0].Type != CredentialMaterialType.HmacSecret)
        {
            context.AddFailure("Keys[0].Type", "Invalid key type for HMAC authentication.");
        }
    }

    private static void ValidateRsa(
        CreateCredentitalSettingCommand command,
        ValidationContext<CreateCredentitalSettingCommand> context)
    {
        if (command.Keys.Length == 0)
            return;

        if (command.Keys.Length != 2)
        {
            context.AddFailure("Keys", "RSA authentication requires exactly 2 keys (public & private).");
            return;
        }

        var types = command.Keys.Select(x => x.Type).ToArray();

        if (!types.Contains(CredentialMaterialType.EcdsaPublicKey) ||
            !types.Contains(CredentialMaterialType.EcdsaPrivateKey))
        {
            context.AddFailure(
                "Keys",
                "RSA authentication requires both public and private keys.");
        }
    }
}