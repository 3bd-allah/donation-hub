using FluentValidation;

namespace DonationHub.Application.Features.Categories.Create;

public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.NameAr)
            .NotEmpty().WithMessage("Name Arabic can't be empty");

        RuleFor(x => x.NameEn)
        .NotEmpty().WithMessage("Name English can't be empty");

    }
}
