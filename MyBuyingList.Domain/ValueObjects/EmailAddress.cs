using MyBuyingList.Domain.Exceptions;
using System.Text.RegularExpressions;
using MyBuyingList.Domain.Common;

namespace MyBuyingList.Domain.ValueObjects;

public class EmailAddress : ValueObject
{
    private static readonly Regex EmailRegex = new("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$");

    public string Address { get; }
    
    public EmailAddress(string address)
    {
        if (!IsValid(address))
            throw new InvalidValueException($"Invalid email address: {address}");

        Address = address;
    }

    private static bool IsValid(string? emailAddress)
    {
        if (string.IsNullOrWhiteSpace(emailAddress))
            return false;

        return EmailRegex.IsMatch(emailAddress);
    }

    // Implicit conversion from string to EmailAddress for convenient usage like: EmailAddress e = "a@b.com";
    public static implicit operator EmailAddress(string emailAddress) 
        => new(emailAddress);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Address;
    }
    
    public override string ToString() => Address;
}
