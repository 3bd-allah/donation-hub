using DonationHub.Application.Abstractions.Persistence;

namespace DonationHub.Application.Behaviors;

public class TransactionContext : ITransactionContext
{
    public bool HasTransactionContext { get; set; }
}
