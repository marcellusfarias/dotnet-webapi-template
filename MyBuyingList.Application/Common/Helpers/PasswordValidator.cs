using System.Text.RegularExpressions;

namespace MyBuyingList.Application.Common.Helpers;

public static class PasswordValidator
{
    private static readonly Regex AllowedCharsRegex = new("[a-zA-Z0-9@#$%&*+_()':;?.,![\\]\\-]+");
    private static readonly Regex DigitRegex = new("[0-9]+");
    private static readonly Regex SpecialCharRegex = new("[@#$%&*+_()':;?.,![\\]\\-]+");
    private static readonly Regex LowerCaseRegex = new("[a-z]+");
    private static readonly Regex UpperCaseRegex = new("[A-Z]+");

    public static bool IsValidPassword(string password)
    {
        // a sequence of one or more characters drawn from the set consisting of ASCII letters, digits or the punctuation characters
        if (!AllowedCharsRegex.IsMatch(password))
            return false;

        // at least one of which is a decimal digit
        if (!DigitRegex.IsMatch(password))
            return false;

        // at least one of which is one of the special characters
        if (!SpecialCharRegex.IsMatch(password))
            return false;

        // at least one of which is a lower-case letter
        if (!LowerCaseRegex.IsMatch(password))
            return false;

        // at least one of which is an upper-case letter
        if (!UpperCaseRegex.IsMatch(password))
            return false;

        // Maximum is 72 characters because BCrypt silently truncates input beyond 72 bytes.
        // All allowed characters are ASCII (1 byte each), so 72 chars == 72 bytes with no truncation risk.
        // This also satisfies the NIST SP 800-63B recommendation of accepting at least 64 characters.
        return password.Length is >= 8 and <= 72;
    }
}
