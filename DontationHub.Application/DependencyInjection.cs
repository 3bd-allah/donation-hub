using DonationHub.Application.Behaviors;
using DonationHub.Application.Options;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;


namespace DonationHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Adding MediatR and its pipelines
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly)
            .AddOpenBehavior(typeof(ValidationBehavior<,>))
        );

        // Adding validators of Application layer
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);


        return services;
    }
}
