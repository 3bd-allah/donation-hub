using DonationHub.Domain.Constants;
using DonationHub.Domain.Enums;
using FluentValidation;

namespace DonationHub.Application.Features.Users.Commands.CreateUser;

public sealed class CreateNewUserCommandValidator : AbstractValidator<CreateNewUserCommand>
{
    public CreateNewUserCommandValidator()
    {
        RuleFor(x => x.Email)
           .NotEmpty().WithMessage("Email is required.")
           .MaximumLength(256).WithMessage("email cannot be more than 256 chars.")
           .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(WebApplicationConstants.PasswordMinimumLength).WithMessage("Password must be at least 6 characters long.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role of user is requeired.");

        RuleFor(x => x.PreferredLanguage)
            .NotEmpty().WithMessage("Preferred Language is required.")
            .Must(pr => pr == "en" || pr == "ar");
    }
}
