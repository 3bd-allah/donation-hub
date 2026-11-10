namespace DonationHub.Api.ViewModels.Authentication.Responses;

public sealed record CurrentProfileViewModel(Guid Id, string Email, string FullName, IEnumerable<string> Roles);
