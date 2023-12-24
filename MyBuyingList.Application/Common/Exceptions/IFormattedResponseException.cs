using MyBuyingList.Web;

namespace MyBuyingList.Application.Common.Exceptions;

public interface IFormattedResponseException
{
    int StatusCode { get; }

    // Nullable in case we dont want error details. Ex: NotFound response.
    ErrorModel? Error { get; }
}
