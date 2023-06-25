using FluentValidation;
using System.Text;

namespace MyBuyingList.Application.Common.Exceptions;

//should this kind of exception be on Domain or Application layer?
public class CustomValidationException : Exception
{
    private static string defaultErrorMessage = "An error occured while validating the model. Ex: {0}";
    public CustomValidationException(ValidationException inner) : base(string.Format(defaultErrorMessage, inner.Message), inner) { }
}
