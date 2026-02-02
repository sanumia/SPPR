using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Lab1.UI.Middleware
{
    public class ErrorRequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorRequestLoggingMiddleware> _logger;

        public ErrorRequestLoggingMiddleware(
            RequestDelegate next,
            ILogger<ErrorRequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Пропускаем запрос дальше по конвейеру
            await _next(context);

            // Проверяем статус код ответа после выполнения запроса
            var statusCode = context.Response.StatusCode;

            // Логируем только если статус код не 2XX
            if (statusCode < 200 || statusCode >= 300)
            {
                var url = context.Request.Path + context.Request.QueryString;
                _logger.LogInformation("---> request {Url} returns {StatusCode}", url, statusCode);
            }
        }
    }
}