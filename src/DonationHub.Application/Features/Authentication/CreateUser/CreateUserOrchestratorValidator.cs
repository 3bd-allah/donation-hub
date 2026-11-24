using DonationHub.Application.Shared.Validation;
using DonationHub.Domain.Enums;
using FluentValidation;

namespace DonationHub.Application.Features.Authentication.CreateUser;

public sealed class CreateUserOrchestratorValidator : AbstractValidator<CreateUserOrchestrator>
{

    public CreateUserOrchestratorValidator()
    {
        RuleFor(x => x.Email)
           .NotEmpty().WithMessage("Email is required.")
           .MaximumLength(256).WithMessage("email cannot be more than 256 chars.")
           .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.");

        RuleFor(x => x.Password)
            .ApplyPasswordRules();
            
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm password is required.")
            .Equal(x => x.Password).WithMessage("Passwords do not match.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role of user is requeired.")
            .IsEnumName(typeof(Roles), false).WithMessage("User role must be valid role");

        RuleFor(x => x.PreferredLanguage)
            .NotEmpty().WithMessage("Preferred Language is required.")
            .IsEnumName(typeof(PreferredLanguage), false).WithMessage("Preferred Language should be valid language.");
    }

}
