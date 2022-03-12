using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Acme.ShoppingCart.WebApi {
    public static class HttpHelper {
        public static string BuildUriFromRequest(HttpRequest request) {
            string reqUrl = "";
            if (request != null) {
                var displayUrl = request?.GetDisplayUrl();
                Uri uri = new Uri(displayUrl);
                if (request.Headers?.ContainsKey("x-forwarded-proto") == true) {
                    var url = request.Headers["x-forwarded-host"].FirstOrDefault();
                    if (string.IsNullOrEmpty(url) || url == "...") {
                        url = request.Host.Value;
                    }
                    var proto = request.Headers["x-forwarded-proto"].FirstOrDefault();
                    var path = uri.AbsolutePath;
                    reqUrl = proto + "://" + url + path;
                } else {
                    reqUrl = uri.ToString();
                }
            }
            return reqUrl;
        }
    }
}
