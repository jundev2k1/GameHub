namespace game_x.application.Extensions;

public static class CommonValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> IsPassword<T>(this IRuleBuilder<T, string> ruleBuilder, string fieldName = "Password")
    {
        return ruleBuilder
            .MinimumLength(8).WithMessage($"{fieldName} must be at least 8 characters")
            .Matches("[A-Z]").WithMessage($"{fieldName} must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage($"{fieldName} must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage($"{fieldName} must contain at least one number")
            .Matches(@"[\!\@\#\$\%\^\&\*\(\)\-\+]").WithMessage($"{fieldName} must contain at least one special character");
    }

    public static IRuleBuilderOptions<T, string> IsEmail<T>(this IRuleBuilder<T, string> ruleBuilder, string fieldName = "Email")
    {
        return ruleBuilder
            .EmailAddress().WithMessage($"{fieldName} format is invalid");
    }

    public static IRuleBuilderOptions<T, string> IsNumber<T>(this IRuleBuilder<T, string> ruleBuilder, string fieldName)
    {
        return ruleBuilder.Matches("^[0-9]+$")
            .WithMessage($"\"{fieldName}\" only digits are allowed.");
    }

    public static IRuleBuilderOptions<T, string> IsPhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder, string fieldName)
    {
        return ruleBuilder
            .Matches(@"^\+?[0-9]{7,15}$").WithMessage($"\"{fieldName}\" format is invalid.");
    }
}
