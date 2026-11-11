using DonationHub.Application;
using DonationHub.Infrastructure;
using DonationHub.Infrastructure.Persistence;
using DonationHub.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace DonationHub.Api;
public class Program
{
    public static async Task Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddProblemDetails();

        builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructureServices(builder.Configuration);


        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

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


        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
