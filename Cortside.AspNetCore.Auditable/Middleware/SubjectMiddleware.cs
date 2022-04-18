using Cortside.Common.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cortside.AspNetCore.Middleware
{
    public class SubjectMiddleware
    {
        private readonly ILogger<SubjectMiddleware> logger;
        private readonly RequestDelegate next;

        public SubjectMiddleware(ILogger<SubjectMiddleware> logger, RequestDelegate next)
        {
            this.logger = logger;
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var subject = SubjectPrincipal.From(context.User);
            var clientId = subject.FindFirst("client_id")?.Value;

            var properties = new Dictionary<string, object>
            {
                ["ClientId"] = clientId ?? string.Empty,
                ["UserPrincipalName"] = subject.UserPrincipalName,
                ["SubjectId"] = subject.SubjectId ?? "anonymous"
            };

            using (logger.BeginScope(properties))
            {
                await next(context);
            }
        }
    }
}
