using DonationHub.Application.Abstractions.Messaging;
using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Features.Categories.Common;
using DonationHub.Application.Shared.ResultPattern;
using DonationHub.Domain.Entities;

namespace DonationHub.Application.Features.Categories.Queries.IsExist;

public sealed record IsCategoryExistQuery(Guid Id) : IQuery<RequestResponse>;

public sealed class IsCategoryExistQueryHandler(IBaseRepository<Category> repository)
    : IQueryHandler<IsCategoryExistQuery, RequestResponse>
{
    public async Task<RequestResponse> Handle(IsCategoryExistQuery request, CancellationToken cancellationToken)
    {

        var isExist = await repository.AnyAsync(c => c.Id == request.Id, cancellationToken);

        return isExist ?
            RequestResponse.Ok() :
            CategoryErrors.NotFound;
    }
}