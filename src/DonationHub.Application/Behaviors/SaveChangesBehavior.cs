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

        // if the result success and flag to indicate that's command is not in a transcation.
        if(response.Success && !transactionContext.HasTransactionContext ) 
        {
            // called for every command but didn't when this command in a transaction, the Transaction Behavior handle this
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return response;
    }
}
