using DonationHub.Domain.Constants;
using DonationHub.Domain.Enums;

namespace DonationHub.Api.ViewModels.Admin.Requests;

public sealed record CreateUserRequestViewModel(
    string Email,
    string FullName,
    string Password,
    string ConfirmPassword,
    string Role,
    string? PreferredLanguage = WebApplicationConstants.DefaultPreferredLanguage);
