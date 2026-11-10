using DonationHub.Application.Shared;
using MediatR;

namespace DonationHub.Application.Features.Authentication.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<RequestResponse<LoginResponseDto>>;
