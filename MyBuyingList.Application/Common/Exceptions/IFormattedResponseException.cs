namespace MyBuyingList.Application.Common.Exceptions;

public interface IFormattedResponseException
{
    int StatusCode { get; }

    // Nullable in case we don't want error details. Ex: NotFound response.
    ErrorModel? Error { get; }
}
