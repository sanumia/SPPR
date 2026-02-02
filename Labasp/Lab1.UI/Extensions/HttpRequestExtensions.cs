using System;
using Microsoft.AspNetCore.Http;

namespace Lab1.UI.Extensions
{
    public static class HttpRequestExtensions
    {
        private const string XmlHttpRequestHeader = "XMLHttpRequest";
        private const string AjaxHeaderName = "X-Requested-With";

        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (!request.Headers.TryGetValue(AjaxHeaderName, out var value))
            {
                return false;
            }

            return string.Equals(value, XmlHttpRequestHeader, StringComparison.OrdinalIgnoreCase);
        }
    }
}






