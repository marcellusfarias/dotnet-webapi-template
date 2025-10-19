using System.Text.RegularExpressions;

namespace MyBuyingList.Application.Common.Helpers;

public static class PasswordHelper
{
    public static bool IsValidPassword(string password)
    {
        // a sequence of one or more characters drawn from the set consisting of ASCII letters, digits or the punctuation characters
        if (!Regex.IsMatch(password, "[a-zA-Z0-9@#$%&*+_()':;?.,![\\]\\-]+")) 
            return false;

        // at least one of which is a decimal digit
        if (!Regex.IsMatch(password, "[0-9]+"))
            return false;

        // at least one of which is one of the special characters
        if (!Regex.IsMatch(password, "[@#$%&*+_()':;?.,![\\]\\-]+"))
            return false;

        // at least one of which is an lower-case letter
        if (!Regex.IsMatch(password, "[a-z]+"))
            return false;

        // at least one of which is an upper-case letter
        if (!Regex.IsMatch(password, "[A-Z]+"))
            return false;

        return password.Length is >= 8 and <= 32;
    }
}
