using DonationHub.Application.Abstractions.Messaging;
using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Shared.ResultPattern;
using MediatR;

namespace DonationHub.Application.Behaviors;

// this behavior will apply for every ITransaction type 
public class TransactionBehavior <TRequest, TResponse> ( IUnitOfWork unitOfWork, ITransactionContext transactionContext )
    : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : ITransaction<TResponse>
    where TResponse : RequestResponse
{
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        
        await unitOfWork.BeginTransactionAsync(cancellationToken);

        transactionContext.HasTransactionContext = true;

        try
        {
            var response = await next(cancellationToken);
            if (response.Success)
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitAsync(cancellationToken);
                return response;
            }
            else
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return response;
            }
        }
        finally
        {
            transactionContext.HasTransactionContext = false;
        }


    }
}
