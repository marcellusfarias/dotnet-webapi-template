using System.Net;

namespace MyBuyingList.Application.Common.Exceptions;

public class AccountLockedException : Exception, IFormattedResponseException
{
    private const string Title = "Account for user {0} is temporarily locked.";
    private const string Details = "Too many failed login attempts. Your account is locked for {0} more minute(s). Please try again later.";
    public int StatusCode => (int)HttpStatusCode.Unauthorized;
    public ErrorResponse Error { get; }
    public int MinutesRemaining { get; }

    public AccountLockedException(string username, int minutesRemaining) : base(string.Format(Title, username))
    {
        MinutesRemaining = minutesRemaining;
        Error = ErrorResponse.CreateSingleErrorDetail(
            string.Format(Title, username),
            string.Format(Details, minutesRemaining));
    }
}
