using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Core.Handler
{

    public abstract class AbstractExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AbstractExceptionHandlerMiddleware> _logger;

        public abstract (HttpStatusCode code, string message) GetResponse(Exception exception);

        public AbstractExceptionHandlerMiddleware(RequestDelegate next, ILogger<AbstractExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        private async Task ErrorResponse(HttpContext context, int statusCode, string message)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = statusCode;
            await response.WriteAsync(message);
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // log the error
                _logger.LogError(ex, "error during executing {Context}", context.Request.Path.Value);

                if (context.Response.HasStarted)
                {
                    throw;
                }
                // get the response code and message
                var (status, message) = GetResponse(ex);
                await ErrorResponse(context, (int)status, message);
            }
        }
    }
}
