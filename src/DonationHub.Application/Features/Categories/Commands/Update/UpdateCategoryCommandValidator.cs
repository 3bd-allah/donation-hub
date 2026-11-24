using FluentValidation;

namespace DonationHub.Application.Features.Categories.Commands.Update;

public sealed class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.NameAr)
            .NotEmpty().WithMessage("Name Arabic can't be empty")
            .MinimumLength(5).WithMessage("min length of name is 5")
            .MaximumLength(100).WithMessage("max length of name is 100");

        RuleFor(x => x.NameEn)
           .NotEmpty().WithMessage("Name English can't be empty")
           .MinimumLength(5).WithMessage("min length of name is 5")
           .MaximumLength(100).WithMessage("max length of name is 100");

        RuleFor(x => x.DescriptionAr)
            .MaximumLength(200).WithMessage("max length of description is 200");

        RuleFor(x => x.DescriptionEn)                                   
            .MaximumLength(200).WithMessage("max length of description is 200");
    }
}
