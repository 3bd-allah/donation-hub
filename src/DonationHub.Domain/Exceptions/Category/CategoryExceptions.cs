namespace DonationHub.Domain.Exceptions.Category;

public class CategoryExceptions(string code, string title, ExceptionType type)
    : DomainException(title, code, type)
{

    public static DomainException Validation(string message)
        => new("Category.Validation", message, ExceptionType.Validation);

}
