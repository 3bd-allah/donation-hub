using DonationHub.Domain.Constants;
using FluentValidation;

namespace DonationHub.Application.Shared.Validation;

public static class PasswordValidationExtension 
{

    public static IRuleBuilderOptions<T, string> ApplyPasswordRules<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
            .WithMessage("Password is required.")

            // options.Password.RequiredLength = WebApplicationConstants.PasswordMinimumLength;
            .MinimumLength(WebApplicationConstants.PasswordMinimumLength)
            .WithMessage($"Password must be at least {WebApplicationConstants.PasswordMinimumLength} characters long.")

            // options.Password.RequireUppercase = true;
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter.")

            // options.Password.RequireLowercase = true;
            .Matches("[a-z]")
            .WithMessage("Password must contain at least one lowercase letter.")

            // options.Password.RequireDigit = true;
            .Matches("[0-9]") // or @"\d"
            .WithMessage("Password must contain at least one digit.")

            // options.Password.RequiredUniqueChars = 1;
            .Matches(@"[!@#$%^&*(),.?""':{}|<>]")
            .WithMessage("Password must contain at least 1 unique character.");
    }
}
