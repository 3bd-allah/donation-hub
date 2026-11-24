using DonationHub.Application.Shared.ResultPattern;
using MediatR;

namespace DonationHub.Application.Abstractions.Messaging;

public interface ICommand <out TResponse> 
    : IRequest<TResponse> 
    where TResponse : RequestResponse
{
}

public interface ICommandHandler<in TRequest, TResponse> 
    : IRequestHandler<TRequest, TResponse> 
    where TRequest : ICommand<TResponse>
    where TResponse : RequestResponse
{

}