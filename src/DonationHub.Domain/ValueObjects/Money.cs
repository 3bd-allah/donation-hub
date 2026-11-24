using DonationHub.Domain.Constants;
using DonationHub.Domain.Enums;
using DonationHub.Domain.Exceptions;
using DonationHub.Domain.Exceptions.Money;
using DonationHub.Domain.Primitives;

namespace DonationHub.Domain.ValueObjects;

public sealed record Money : ValueObject
{
    public decimal Amount { get; private set; } 
    public Currency Currency { get; private set; }

    private Money(){ }
    public Money(decimal amount, string currency = WebApplicationConstants.DefaultCurrency)
    {
        if (amount < 0)
            throw MoneyExceptions.InvalidAmount;
        Amount = amount;

        if(string.IsNullOrWhiteSpace(currency) ||
        !Enum.TryParse<Currency>(currency, ignoreCase: true, out var currencyResult) ||
        !Enum.IsDefined(currencyResult))
            throw MoneyExceptions.Currency("Currency must be a valid currency code.");
        Currency = currencyResult;
    }

    private Money(decimal amount, Currency currency )
    {
        if (amount < 0)
            throw MoneyExceptions.InvalidAmount;
        Amount = amount;
        
        Currency = currency;
    }
    public static Money operator  + (Money a, Money b)
    {
        EnsureSameCurrencies(a, b);

        return new Money(a.Amount + b.Amount, a.Currency);
    }

    public static Money operator  - (Money a, Money b)
    {
        EnsureSameCurrencies(a, b);
        return new Money(a.Amount - b.Amount, a.Currency);
    }

    public static bool operator > (Money a, Money b)
    {
        EnsureSameCurrencies(a, b);

        return a.Amount > b.Amount;
    }

    public static bool operator < (Money a, Money b)
    {
        EnsureSameCurrencies(a, b);
        return a.Amount < b.Amount;
    }

    public static bool operator >= (Money a, Money b)
    {
        EnsureSameCurrencies(a, b);
        return a.Amount >= b.Amount;
    }

    public static bool operator <= (Money a, Money b)
    {
        EnsureSameCurrencies(a, b);
        return a.Amount <= b.Amount;
    }


    public static Money operator / (Money a, Money b)
    {
        EnsureSameCurrencies(a, b);
        return new Money(a.Amount / b.Amount, a.Currency);
    }

    public static Money operator * (Money a, Money b)
    {
        EnsureSameCurrencies(a, b);
        return new Money(a.Amount * b.Amount, a.Currency);
    }
    private static void EnsureSameCurrencies(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw MoneyExceptions.Currency("Cannot use Money amounts with different currencies.");
    }
}
