using DonationHub.Domain.Constants;
using FluentValidation;

namespace DonationHub.Application.Features.Authentication.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(WebApplicationConstants.PasswordMinimumLength).WithMessage("Password must be more than 8 chars.");
    }
}
