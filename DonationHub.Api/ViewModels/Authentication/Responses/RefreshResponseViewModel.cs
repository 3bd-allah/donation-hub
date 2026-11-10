using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DonationHub.Api.ViewModels.Authentication.Responses;

public sealed record RefreshResponseViewModel(
    string AccessToken,
    DateTimeOffset ExpiresAt,
    string RefreshToken);
