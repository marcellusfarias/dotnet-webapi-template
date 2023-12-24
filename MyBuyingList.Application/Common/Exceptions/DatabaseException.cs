﻿using MyBuyingList.Web;
using System.Net;

namespace MyBuyingList.Application.Common.Exceptions;

public class DatabaseException : Exception, IFormattedResponseException
{
    private readonly static string _responseTitle = "An database operation error occured.";
    
    public int StatusCode => (int)HttpStatusCode.InternalServerError;
    public ErrorModel Error { get; private set; }

    public DatabaseException(Exception inner) : base(string.Format(_responseTitle), inner)
    {
        Error = ErrorModel.CreateSingleErrorDetailsModel(_responseTitle, "Please, contact administrator");
    }
}
