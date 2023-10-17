using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Acme.IdentityServer.WebApi {
    public static class ILoggerExtensions {
        public static void ForContext<T>(this ILogger logger, Action LogAction) {
            Dictionary<string, object> contextData = new Dictionary<string, object>();
            contextData.TryAdd("SourceContext", nameof(T));

            using (logger.BeginScope(contextData)) {
                LogAction.Invoke();
            }
        }

        public static void ForContext(this ILogger logger, string sourceContext, Action LogAction) {
            Dictionary<string, object> contextData = new Dictionary<string, object>();
            contextData.TryAdd("SourceContext", sourceContext);

            using (logger.BeginScope(contextData)) {
                LogAction.Invoke();
            }
        }

        public static void LogWithContext(this ILogger logger, Action LogAction,
            params KeyValuePair<string, object>[] contextDataParam) {
            Dictionary<string, object> contextData = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> kvp in contextDataParam) {
                contextData.TryAdd(kvp.Key, kvp.Value);
            }

            using (logger.BeginScope(contextData)) {
                LogAction.Invoke();
            }
        }
    }
}
