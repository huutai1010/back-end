using BLL.Responses;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Exceptions
{
    public class BaseException : BaseResponse
    {
        public string Message { get; set; }
        public IDictionary<string, string[]>? Errors { get; set; }
        public BaseException() : base()
        {
            StatusCode = (int)HttpStatusCode.InternalServerError;
            Message = "System has issues!";
        }

        public BaseException(int statusCode, string message) : base(statusCode)
        {
            Message = message;
        }
        public BaseException(string message, IDictionary<string, string[]> validationErrors) : base((int)HttpStatusCode.BadRequest)
        {
            Message = message;
            Errors = validationErrors;
        }
    }
}
