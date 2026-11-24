using DonationHub.Application.Shared.Validation;
using DonationHub.Domain.Constants;
using FluentValidation;

namespace DonationHub.Application.Features.Authentication.Login;

public sealed class LoginOrchestratorValidator : AbstractValidator<LoginOrchestrator>
{
    public LoginOrchestratorValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .ApplyPasswordRules();
    }
}
