
using BLL.Exceptions;

using Newtonsoft.Json;

using System.Net;

namespace API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            BaseException problem;

            switch (ex)
            {
                case ForbiddenException forbiddenException:
                    statusCode = HttpStatusCode.Forbidden;
                    problem = new BaseException
                    {
                        StatusCode = (int)statusCode,
                        Message = forbiddenException.Message,
                    };
                    break;
                case BadRequestException badRequestException:
                    statusCode = HttpStatusCode.BadRequest;
                    problem = new BaseException
                    {
                        StatusCode = (int)statusCode,
                        Message = badRequestException.Message,
                        Errors = badRequestException.ValidationErrors,
                    };
                    break;
                case NotFoundException notFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    problem = new BaseException
                    {
                        StatusCode = (int)statusCode,
                        Message = notFoundException.Message,
                    };
                    break;
                default:
                    problem = new BaseException();
                    // problem.Message = ex.Message;
                    break;
            }

            httpContext.Response.StatusCode = (int)statusCode;
            var logMessage = JsonConvert.SerializeObject(problem);
            _logger.LogError(logMessage);
            await httpContext.Response.WriteAsJsonAsync(problem);

        }
    }
}
