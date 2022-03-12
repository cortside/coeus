using System.Threading.Tasks;
using Cortside.Common.Security;
using Microsoft.AspNetCore.Http;

namespace Acme.ShoppingCart.WebApi.Middleware {
    public class SubjectMiddleware {
        private readonly RequestDelegate _next;

        public SubjectMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context) {
            var subject = SubjectPrincipal.From(context.User);
            var clientId = subject.FindFirst("client_id")?.Value;

            using (Serilog.Context.LogContext.PushProperty("ClientId", clientId))
            using (Serilog.Context.LogContext.PushProperty("UserPrincipalName", subject.UserPrincipalName))
            using (Serilog.Context.LogContext.PushProperty("SubjectId", subject.SubjectId ?? "anonymous")) {
                await _next(context);
            }
        }
    }
}
