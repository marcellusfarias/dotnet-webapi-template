namespace MyBuyingList.Application.Common.Exceptions;

/// <summary>
/// Interface for exceptions that provide formatted HTTP error responses.
/// </summary>
public interface IFormattedResponseException
{
    /// <summary>
    /// The HTTP status code to return.
    /// </summary>
    int StatusCode { get; }

    /// <summary>
    /// The error details to include in the response body.
    /// Null when no error details are needed (e.g., NotFound responses).
    /// </summary>
    ErrorModel? Error { get; }
}
