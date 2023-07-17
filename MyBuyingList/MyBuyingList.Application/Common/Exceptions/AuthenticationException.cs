using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Application.Common.Exceptions;

public class AuthenticationException : Exception
{
    private static string defaultErrorMessage = "An error occured when authenticating user {0}.";
    public AuthenticationException(Exception inner, string username) : base(string.Format(defaultErrorMessage, username), inner) { }
}