using DonationHub.Domain.Constants;
using DonationHub.Domain.Primitives;

namespace DonationHub.Domain.ValueObjects;

public sealed record Money : ValueObject
{
    private const int DefaultLength = 3;
    public decimal Amount{ get; private set; }
    public string Currency { get; set; } = WebApplicationConstants.DefaultCurrency;


    public Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentException("Value cannot be negative.", nameof(amount));
        Amount = amount;

        if(string.IsNullOrEmpty(currency) || currency.Length != DefaultLength)
            throw new ArgumentException($"Currency must be a {DefaultLength}-letter code.", nameof(currency));
        Currency = currency;
    }

    public static Money operator  + (Money a, Money b)
    {
        if(a.Currency != b.Currency)
            throw new InvalidOperationException("Cannot add Money values with different currencies.");

        return new Money(a.Amount + b.Amount, a.Currency);
    }

    public static Money operator  - (Money a, Money b)
    {
        if(a.Currency != b.Currency)
            throw new InvalidOperationException("Cannot subtract Money values with different currencies.");

        return new Money(a.Amount - b.Amount, a.Currency);
    }

    public static bool operator  > (Money a, Money b)
    {
        if(a.Currency != b.Currency)
            throw new InvalidOperationException("Cannot compare Money values with different currencies.");

        return a.Amount > b.Amount;
    }

    public static bool operator <(Money a, Money b)
    {
        if(a.Currency != b.Currency)
            throw new InvalidOperationException("Cannot compare Money values with different currencies.");

        return a.Amount < b.Amount;
    }

}
