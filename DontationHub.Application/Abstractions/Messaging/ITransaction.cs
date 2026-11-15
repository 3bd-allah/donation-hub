using DonationHub.Application.Shared.ResultPattern;
using MediatR;

namespace DonationHub.Application.Abstractions.Messaging;

public interface ITransaction<out TResponse> : ICommand<TResponse> 
    where TResponse : RequestResponse
{
}

public interface ITransactionHandler <in TTransaction, TResponse> 
    : IRequestHandler<TTransaction, TResponse> 
    where TTransaction : ITransaction<TResponse> 
    where TResponse : RequestResponse
{

}