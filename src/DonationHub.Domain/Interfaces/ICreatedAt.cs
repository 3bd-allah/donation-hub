using DonationHub.Domain.Entities;

namespace DonationHub.Domain.Interfaces;

public interface ICreatedAt
{
    DateTimeOffset CreatedAt { get; set; }

}
