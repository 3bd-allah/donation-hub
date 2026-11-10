namespace DonationHub.Api.ViewModels.Authentication.Requests
{
    public sealed record RegisterRequestViewModel(
        string Email,
        string FullName,
        string Password,
        string ConfirmPassword,
        string? PreferredLanguage);
}
