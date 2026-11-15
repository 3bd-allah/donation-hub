namespace DonationHub.Application.Abstractions.Persistence;

public interface ITransactionContext
{
    bool HasTransactionContext { get; set; }
}
