using DonationHub.Application.Abstractions.Authentication;
using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Options;
using DonationHub.Domain.Constants;
using DonationHub.Infrastructure.Authentication;
using DonationHub.Infrastructure.Identity;
using DonationHub.Infrastructure.Interceptors;
using DonationHub.Infrastructure.Options;
using DonationHub.Infrastructure.Persistence;
using DonationHub.Infrastructure.Persistence.Seed;
using DonationHub.Infrastructure.Repositories;
using DonationHub.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace DonationHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {

        

        // Timezone configuration
        services.AddSingleton<TimeProvider>(TimeProvider.System);

        // Add Auth services
        services.AddScoped<IRegisterService, RegisterService>();
        services.AddSingleton<ITokenService, TokenService>();
        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<IUserService, UserService>();

        // Add Interceptors
        services.AddScoped<SoftDeleteInterceptor>();
        services.AddScoped<AuditableInterceptor>();

        // Add DbContext and Identity services
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.UseSqlServer(configuration.GetConnectionString(ConnectionStringOptions.SqlServerConnectionStringName));
            // adding the interceptor from the IOC container to the DbContext options

            // Interceptors
            IEnumerable<IInterceptor> interceptors = [
                sp.GetRequiredService<SoftDeleteInterceptor>(),
                sp.GetRequiredService<AuditableInterceptor>()
            ];

            options.AddInterceptors(interceptors);
        });

        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.Password.RequiredLength = WebApplicationConstants.PasswordMinimumLength;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireDigit = true;
            options.User.RequireUniqueEmail = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredUniqueChars = 1;
        })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddUserStore<UserStore<ApplicationUser, ApplicationRole, AppDbContext, Guid>>()
            .AddRoleStore<RoleStore<ApplicationRole, AppDbContext, Guid>>();

        //Add Database Seeder
        services.AddScoped<DatabaseSeeder>();


        // Add Options
        services.AddOptions<ConnectionStringOptions>()
            .Bind(configuration.GetSection(ConnectionStringOptions.ConnectionStringSettings))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.JwtSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddOptions<SeedAdminOptions>()
            .BindConfiguration(SeedAdminOptions.SeedAdminSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<DonationOptions>()
            .BindConfiguration(DonationOptions.DonationSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Add Repositories
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();


        //Add Authentication and Authorization
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        }).AddJwtBearer(options =>
        {
            options.MapInboundClaims = false;

            //validate jwt token claims 
            options.TokenValidationParameters = new TokenValidationParameters
            {
                // role claim
                RoleClaimType = "role",

                // audience claim
                ValidateAudience = true,
                ValidAudience = configuration[JwtOptions.AudienceKey],

                // issuer claim
                ValidateIssuer = true,
                ValidIssuer = configuration[JwtOptions.IssuerKey],

                // validate secret key 
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes(configuration[JwtOptions.SecretKey]!)),

                ClockSkew = TimeSpan.Zero,  

                ValidateLifetime = true     
            };
        });

        

        return services;
    }
}
