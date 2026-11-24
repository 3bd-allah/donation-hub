namespace DonationHub.Application.Abstractions.Persistence;

public interface ITransactionContext
{
    // flag to indicate that the command is in a transaction or not to save database round trips when calling SaveChanges
    bool HasTransactionContext { get; set; }
}
