using Microsoft.AspNetCore.Mvc.Controllers;
using Serilog.Context;

namespace MizanERP.Web.Middleware
{
    public class RequestContextMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var action = endpoint?.Metadata
                .GetMetadata<ControllerActionDescriptor>();

            using var c1 = LogContext.PushProperty("Controller", action?.ControllerName ?? "Unknown");
            using var c2 = LogContext.PushProperty("Action", action?.ActionName ?? "Unknown");
            using var c3 = LogContext.PushProperty("User", context.User?.Identity?.Name ?? "UnKnown");

            await _next(context);
        }
    }
}
