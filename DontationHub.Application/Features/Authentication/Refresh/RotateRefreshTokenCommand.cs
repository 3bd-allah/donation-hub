using DonationHub.Application.Shared;
using MediatR;

namespace DonationHub.Application.Features.Authentication.Refresh;

public sealed record RotateRefreshTokenCommand(string RefreshToken) :
    IRequest<RequestResponse<RotateRefreshTokenResponseDto>>;
