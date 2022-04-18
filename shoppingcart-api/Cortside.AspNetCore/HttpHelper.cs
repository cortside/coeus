using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Cortside.AspNetCore
{
    /// <summary>
    /// Helper class for Http methods
    /// </summary>
    public static class HttpHelper
    {
        /// <summary>
        /// Builds the requesting uri from the request, using proxied headers if present
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public static string BuildUriFromRequest(HttpRequest request)
        {
            if (request == null)
            {
                return string.Empty;
            }

            var displayUrl = request.GetDisplayUrl();
            var uri = new Uri(displayUrl);
            if (request.Headers?.ContainsKey("x-forwarded-proto") == true)
            {
                var url = request.Headers["x-forwarded-host"].FirstOrDefault();
                if (string.IsNullOrEmpty(url) || url == "...")
                {
                    url = request.Host.Value;
                }
                var proto = request.Headers["x-forwarded-proto"].FirstOrDefault();
                var path = uri.AbsolutePath;
                return proto + "://" + url + path;
            }

            return uri.ToString();
        }
    }
}
