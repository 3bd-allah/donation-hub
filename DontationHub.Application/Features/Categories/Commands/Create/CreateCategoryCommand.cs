using DonationHub.Application.Abstractions.Messaging;
using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Features.Categories.Common;
using DonationHub.Application.Shared.ResultPattern;
using DonationHub.Domain.Entities;

namespace DonationHub.Application.Features.Categories.Commands.Create;

public sealed record CreateCategoryCommand(
    string NameAr, 
    string NameEn, 
    string? DescriptionAr, 
    string? DescriptionEn) : ICommand<RequestResponse<Guid>>;


public class CreateCategoryCommandHandler( IBaseRepository<Category> repository ) 
    : ICommandHandler<CreateCategoryCommand, RequestResponse<Guid>>
{
    public async Task<RequestResponse<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        // duplication check
        var duplicationCheck = await repository
            .FirstOrDefaultAsync(c => c.NameAr == request.NameAr || c.NameEn == request.NameEn,
            c => new { c.NameAr, c.NameEn }, cancellationToken);

        if (duplicationCheck is not null || duplicationCheck?.NameAr is not null)
            return CategoryErrors.DuplicateNameAr;

        if (duplicationCheck?.NameEn is not null)
            return CategoryErrors.DuplicateNameEn;

        var category = Category.Create(request.NameAr, request.NameEn, request.DescriptionAr, request.DescriptionEn);
        repository.Add(category);

        return category.Id;
    }
}