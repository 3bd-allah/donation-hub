using DonationHub.Application.Shared;
using DonationHub.Domain.Constants;
using MediatR;

namespace DonationHub.Application.Features.Authentication.Register;

public record RegisterCommand(
    string Email,
    string FullName,
    string Password,
    string ConfirmPassword,
    string PreferredLanguage = WebApplicationConstants.DefaultPreferredLanguage)
    : IRequest<RequestResponse>;
