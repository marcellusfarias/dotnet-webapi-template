using MyBuyingList.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace MyBuyingList.Domain.ValueObjects;

// TODO: replace "string" Email for this ValueObject
public class EmailAddress : ValueObject
{
    private static readonly Regex EmailRegex = new Regex("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$");

    public string Address { get; init; }

    public EmailAddress(string emailAddress)
    {
        if (!IsValid(emailAddress))
            throw new InvalidValueException();

        Address = emailAddress;
    }

    public static bool IsValid(string emailAddress)
    {
        return EmailRegex.IsMatch(emailAddress);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Address;
    }
}
