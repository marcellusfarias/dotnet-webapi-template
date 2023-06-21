using MyBuyingList.Application.Common.Interfaces;

namespace MyBuyingList.Application.Common.Errors;

public class UnexpectedError : IApplicationError
{
    private string _message;
    public UnexpectedError(string message)
        => _message = message;
    public string Message { get { return _message; } }
    public int ErrorCode => 400;
}
