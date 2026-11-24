using DonationHub.Application.Shared.ResultPattern;
using FluentValidation;
using MediatR;

namespace DonationHub.Application.Behaviors;

// this behavior will apply for every command - query and transaction 
public class ValidationBehavior<TReqeust, TResponse>( IEnumerable<IValidator<TReqeust>> validators ) 
    : IPipelineBehavior<TReqeust, TResponse> 
    where TReqeust : IRequest<TResponse>
    where TResponse : RequestResponse, IRequestResponse<TResponse>
{
    public async Task<TResponse> Handle(TReqeust request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TReqeust>(request);

            var result = await Task.WhenAll(
                        validators.Select(async v => await
                        v.ValidateAsync(context, cancellationToken)));

            var failures = result.SelectMany(r => r.Errors)
                .Where(f => f is not null)
                .Select( f => Error.Validation("Validation.Failed", f.ErrorMessage, f.PropertyName))
                .ToArray();

            if (failures.Length > 0)
            {
               return TResponse.Fail(failures);
            }
        }
        return await next(cancellationToken);
    }
}
