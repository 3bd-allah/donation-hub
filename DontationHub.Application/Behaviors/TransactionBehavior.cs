using DonationHub.Application.Abstractions.Messaging;
using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Shared.ResultPattern;
using MediatR;

namespace DonationHub.Application.Behaviors;

public class TransactionBehavior <TRequest, TResponse> ( IUnitOfWork unitOfWork, ITransactionContext transactionContext )
    : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : ITransaction<RequestResponse>
    where TResponse : RequestResponse
{
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        transactionContext.HasTransactionContext = true;

        var response = await next(cancellationToken);

        if (response.Success)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        else
        {
            await transaction.RollbackAsync(cancellationToken);
        }
        

        return response;

    }
}
