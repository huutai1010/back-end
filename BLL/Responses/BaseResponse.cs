using System.Net;

namespace BLL.Responses
{
    public abstract class BaseResponse
    {
        public int StatusCode { get; set; }
        public BaseResponse()
        {
            StatusCode = (int)HttpStatusCode.OK;
        }
        public BaseResponse(int statusCode)
        {
            StatusCode = statusCode; 
        }
    }
}
