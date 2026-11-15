namespace DonationHub.Api.Shared;

public sealed record EndpointError(
    string Code,
    string Description,
    string Type,
    string? Field);

