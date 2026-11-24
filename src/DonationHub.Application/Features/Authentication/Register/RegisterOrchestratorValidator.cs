using DonationHub.Application.Shared.Validation;
using DonationHub.Domain.Constants;
using FluentValidation;

namespace DonationHub.Application.Features.Authentication.Register;

public class RegisterOrchestratorValidator : AbstractValidator<RegisterOrchestrator>
{
    public RegisterOrchestratorValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .MaximumLength(256).WithMessage("email cannot be more than 256 chars.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(100).WithMessage("Full name cannot be more than 100 chars.");

        RuleFor(x => x.Password)
            .ApplyPasswordRules();

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm password is required.")
            .Equal(x => x.Password).WithMessage("Passwords do not match.");
    }
}