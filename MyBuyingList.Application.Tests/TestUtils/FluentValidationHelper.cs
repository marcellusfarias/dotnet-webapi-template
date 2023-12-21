using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Application.Tests.TestUtils;

public static class FluentValidationHelper
{
    public static string GetMinLengthMessage(string fieldName, int minCharacterLenght, int enteredCharacters)
    {
        return $"The length of '{fieldName}' must be at least {minCharacterLenght} characters. You entered {enteredCharacters} characters.";
    }

    public static string GetMaxLengthMessage(string fieldName, int maxCharacterLenght, int enteredCharacters)
    {
        return $"The length of '{fieldName}' must be {maxCharacterLenght} characters or fewer. You entered {enteredCharacters} characters.";
    }

    public static string GetNotEmptyFieldMessage(string fieldName)
    {
        return $"'{fieldName}' must not be empty.";
    }

    public static string GetFieldGreaterThanMessage(string fieldName, int value)
    {
        return $"'{fieldName}' must be greater than '{value}'.";
    }
}
