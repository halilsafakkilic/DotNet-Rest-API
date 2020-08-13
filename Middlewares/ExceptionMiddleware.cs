using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using netCorePlayground.DTOs;
using Newtonsoft.Json;

namespace netCorePlayground.Middlewares
{
    public class ExceptionMiddleware
    {
        private const string DefaultErrorMessage = "A server error occurred.";

        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionMiddleware(RequestDelegate next, ILoggerFactory logger)
        {
            _logger = logger.CreateLogger("ExceptionMiddleware");
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");

                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(JsonConvert.SerializeObject(new ApiError
            {
                Message = DefaultErrorMessage
            }));
        }
    }
}