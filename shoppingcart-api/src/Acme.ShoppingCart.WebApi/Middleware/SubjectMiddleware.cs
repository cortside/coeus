using System.Threading.Tasks;
using Cortside.Common.Security;
using Microsoft.AspNetCore.Http;

namespace Acme.ShoppingCart.WebApi.Middleware {
    public class SubjectMiddleware {
        private readonly RequestDelegate _next;
        private readonly IHttpContextAccessor _httpAccessor;

        public SubjectMiddleware(RequestDelegate next, IHttpContextAccessor httpAccessor) {
            _next = next;
            _httpAccessor = httpAccessor;
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
