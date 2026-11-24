using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Behaviors;
using DonationHub.Application.Options;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Input;


namespace DonationHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Adding MediatR and its pipelines
        services.AddMediatR((cfg) =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

            // Order is matter for pipelines:
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
            cfg.AddOpenBehavior(typeof(SaveChangesBehavior<,>));
        }); 

        // Adding validators of Application layer
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<ITransactionContext, TransactionContext>();


        return services;
    }
}
