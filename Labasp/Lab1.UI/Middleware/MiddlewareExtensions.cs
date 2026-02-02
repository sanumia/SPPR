using Microsoft.AspNetCore.Builder;

namespace Lab1.UI.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorRequestLoggingMiddleware>();
        }
    }
}