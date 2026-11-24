namespace DonationHub.Domain.Exceptions.Money;

public class MoneyExceptions 
{
    public static DomainException InvalidAmount
        => new("Money.InvalidAmount", "Money amount cannot be negative", ExceptionType.UnprocessableEntity);

    public static DomainException Currency(string message)
        => new("Money.InvalidCurrency", message, ExceptionType.UnprocessableEntity);

}
