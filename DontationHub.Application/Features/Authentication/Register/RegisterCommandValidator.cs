using DonationHub.Application.Features.Authentication.Register;
using DonationHub.Domain.Constants;
using FluentValidation;

namespace DonationHub.Infrastructure.Authentication.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(WebApplicationConstants.PasswordMinimumLength).WithMessage("Password must be at least 6 characters long.");
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm password is required.")
            .Equal(x => x.Password).WithMessage("Passwords do not match.");
    }
}