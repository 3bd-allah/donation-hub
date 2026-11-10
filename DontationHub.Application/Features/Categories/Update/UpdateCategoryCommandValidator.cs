using FluentValidation;

namespace DonationHub.Application.Features.Categories.Update;

public sealed class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.NameAr)
            .MaximumLength(100).WithMessage("max length of name is 100");

        RuleFor(x => x.NameEn)
            .MaximumLength(100).WithMessage("max length of name is 100");

        RuleFor(x => x.DescriptionAr)
            .MaximumLength(200).WithMessage("max length of description is 200");

        RuleFor(x => x.DescriptionEn)                                   
            .MaximumLength(200).WithMessage("max length of description is 200");
    }
}
