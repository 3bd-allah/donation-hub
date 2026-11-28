using DonationHub.Api.Exceptions;
using DonationHub.Api.Infrastructure;
using DonationHub.Application;
using DonationHub.Application.Abstractions.Authentication;
using DonationHub.Infrastructure;
using DonationHub.Infrastructure.Persistence;
using DonationHub.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

namespace DonationHub.Api;
public class Program
{
    public static async Task Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
            };
        });

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        builder.Services.AddScoped<ICurrentUser, CurrentUser>();
        builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructureServices(builder.Configuration);


        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi( options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Components ??= new OpenApiComponents();

                document.Components.SecuritySchemes ??=
                    new Dictionary<string, IOpenApiSecurityScheme>();

                document.Components.SecuritySchemes["Bearer"] =
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Description = "Enter your JWT access token."
                    };

                return Task.CompletedTask;
            });

            options.AddOperationTransformer((operation, context, cancellationToken) =>
            {
                var hasAuthorize = context.Description.ActionDescriptor
                    .EndpointMetadata
                    .OfType<IAuthorizeData>()
                    .Any();

                var hasAllowAnonymous = context.Description.ActionDescriptor
                    .EndpointMetadata
                    .OfType<IAllowAnonymous>()
                    .Any();

                if (hasAuthorize && !hasAllowAnonymous)
                {
                    operation.Security ??= [];

                    operation.Security.Add(new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = []
                    });
                }

                return Task.CompletedTask;
            });
        });

        var app = builder.Build();

        app.UseExceptionHandler();
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            //app.UseDeveloperExceptionPage(); // for development debuging 
            await using (var scope = app.Services.CreateAsyncScope())
            {
                var dbContext = scope.ServiceProvider
                    .GetRequiredService<AppDbContext>();

                // Ensure Identity tables exist.
                await dbContext.Database.MigrateAsync();

                var seeder = scope.ServiceProvider
                    .GetRequiredService<DatabaseSeeder>();

                await seeder.SeedAsync();
            }

            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.AddPreferredSecuritySchemes("Bearer");
            });
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
