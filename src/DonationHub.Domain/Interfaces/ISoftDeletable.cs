namespace DonationHub.Domain.Interfaces;

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }

    DateTimeOffset? DeletedAtUTC { get; set; }

    void Delete()
    {
        IsDeleted = true;
    }

    void UndoDelete()
    {
        IsDeleted = false;
        DeletedAtUTC = null;
    }   
}
