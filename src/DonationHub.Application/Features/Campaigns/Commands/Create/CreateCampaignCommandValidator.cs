using DonationHub.Domain.Enums;
using FluentValidation;

namespace DonationHub.Application.Features.Campaigns.Commands.Create;

public sealed class CreateCampaignCommandValidator : AbstractValidator<CreateCampaignCommand>
{
    public CreateCampaignCommandValidator()
    {
        RuleFor(x => x.TitleAr)
            .NotEmpty().WithMessage("Title ar is required.")
            .MaximumLength(100).WithMessage("Title ar cannot be more than 100 chars.");

        RuleFor(x => x.TitleEn)
            .NotEmpty().WithMessage("Title en is required")
            .MaximumLength(100).WithMessage("Title en cannot be more than 100 chars.");

        RuleFor(x => x.DescriptionAr)
            .NotEmpty().WithMessage("Description ar is required.")
            .MaximumLength(500).WithMessage("Description ar cannot be more than 500chars");

        RuleFor(x => x.DescriptionEn)
            .NotEmpty().WithMessage("Description en is required.")
            .MaximumLength(500).WithMessage("Description en cannot be more than 500chars");

        RuleFor(x => x.GoalAmount)
            .NotEmpty().WithMessage("Goal Amount is required.")
            .GreaterThan(0).WithMessage("Goal Amount should be greater than zero.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required.")
            .IsEnumName(typeof(Currency)).WithMessage("Currency should be valid currency code.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End Date is required.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category Id is required.");

        RuleFor(x => x.BannerFileName)
            .MaximumLength(100).WithMessage("Banner file name cannot be more than 100 chars.");

    }
}
