using DonationHub.Application.Shared.ResultPattern;
using MediatR;

namespace DonationHub.Application.Abstractions.Messaging;

public interface IQuery<out TResponse> : IRequest<TResponse> 
    where TResponse : RequestResponse
{
}

public interface IQueryHandler<in TRequest, TResponse> 
    : IRequestHandler<TRequest, TResponse> 
    where TRequest : IQuery<TResponse> 
    where TResponse : RequestResponse
{

}