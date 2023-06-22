using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Application.Common.Exceptions;

public class DatabaseException : Exception
{
    private static string defaultErrorMessage = "An error occured while operating in the database. Ex: {0}";
    public DatabaseException(Exception inner) : base(string.Format(defaultErrorMessage, inner.Message), inner) { }
}
