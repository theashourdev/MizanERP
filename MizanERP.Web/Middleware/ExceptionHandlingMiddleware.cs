using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MizanERP.Web.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string errorType = "ServerError";
            string message = "An unexpected error occurred.";

            if (exception is ArgumentException || exception is InvalidOperationException)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                errorType = "ValidationError";
                message = exception.Message;
            }
            // Add more business exception types as needed

            context.Response.StatusCode = statusCode;
            var result = JsonSerializer.Serialize(new
            {
                error = errorType,
                message,
                details = exception is not InvalidOperationException ? exception.StackTrace : null
            });
            await context.Response.WriteAsync(result);
        }
    }
}