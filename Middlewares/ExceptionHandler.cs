using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using netCorePlayground.DTOs;
using Newtonsoft.Json;

namespace netCorePlayground.Middlewares
{
    [Obsolete]
    public class ExceptionHandler
    {
        private readonly IWebHostEnvironment _environment;

        private const string DefaultErrorMessage = "A server error occurred.";

        public ExceptionHandler(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

            var ex = httpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
            if (ex == null)
            {
                return;
            }

            var error = new ApiError();
            if (_environment.IsDevelopment())
            {
                error.Message = ex.Message;
                error.Detail = ex.StackTrace;
                //error.InnerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
            }
            else
            {
                error.Message = DefaultErrorMessage;
                error.Detail = ex.Message;
            }

            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(error));
        }
    }
}