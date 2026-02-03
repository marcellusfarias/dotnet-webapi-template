using System.Net;

namespace MyBuyingList.Application.Common.Exceptions;

public class ResourceNotFoundException : Exception, IFormattedResponseException
{
    public int StatusCode => (int)HttpStatusCode.NotFound;
    public ErrorResponse? Error => null;
}
