using DonationHub.Application.Shared;
using MediatR;

namespace DonationHub.Application.Features.Authentication.Logout;

public sealed record LogoutCommand(string RefreshToken) : IRequest<RequestResponse>;
