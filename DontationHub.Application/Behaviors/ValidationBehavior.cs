using FluentValidation;
using MediatR;

namespace DonationHub.Application.Behaviors;
public class ValidationBehavior<TReqeust, TResponse>(IEnumerable<IValidator<TReqeust>> validators) 
    : IPipelineBehavior<TReqeust, TResponse> where TReqeust : notnull
{
    public async Task<TResponse> Handle(TReqeust request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TReqeust>(request);

            var result = await Task.WhenAll( 
                    validators.Select(v => 
                        v.ValidateAsync(context, cancellationToken)));

            var failures = result.SelectMany(r => r.Errors)
                .Where(f => f is not null).ToList();

            if (failures.Count > 0)
            {
                throw new ValidationException(failures);
            }
        }
        return await next(cancellationToken);
    }
}
