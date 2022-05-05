using Serilog;
using System;
using System.Text.Json;
using WireMock.Admin.Requests;
using WireMock.Logging;

namespace PolicyServer
{
    public class WireMockLogger : IWireMockLogger
    {
        private readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        private readonly ILogger _logger;

        public WireMockLogger(ILogger logger)
        {
            _logger = logger;
        }

        /// <see cref="IWireMockLogger.Debug"/>
        public void Debug(string formatString, params object[] args)
        {
            _logger.Debug(formatString, args);
        }

        /// <see cref="IWireMockLogger.Info"/>
        public void Info(string formatString, params object[] args)
        {
            _logger.Information(formatString, args);
        }

        /// <see cref="IWireMockLogger.Warn"/>
        public void Warn(string formatString, params object[] args)
        {
            _logger.Warning(formatString, args);
        }

        /// <see cref="IWireMockLogger.Error(string, object[])"/>
        public void Error(string formatString, params object[] args)
        {
            _logger.Error(formatString, args);
        }

        /// <see cref="IWireMockLogger.Error(string, Exception)"/>
        public void Error(string formatString, Exception exception)
        {
            _logger.Error(formatString, exception);
        }

        /// <see cref="IWireMockLogger.DebugRequestResponse"/>
        public void DebugRequestResponse(LogEntryModel logEntryModel, bool isAdminRequest)
        {
            string message = JsonSerializer.Serialize(logEntryModel, options);

            _logger.Debug("Admin[{IsAdmin}] {Message}", isAdminRequest, message);
        }
    }
}
