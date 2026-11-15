using DonationHub.Application.Abstractions.Messaging;
using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Shared.ResultPattern;
using MediatR;

namespace DonationHub.Application.Behaviors;

public sealed class SaveChangesBehavior<TRequest, TResponse> ( 
    IUnitOfWork unitOfWork, 
    ITransactionContext transactionContext) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
    where TResponse : RequestResponse
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {

        var response = await next(cancellationToken);

        if(response is RequestResponse result && result.Success && !transactionContext.HasTransactionContext )
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return response;
    }
}
//  <PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.14" />
