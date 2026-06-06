using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vicgital.MediaOrganizer.Infrastructure.Logging.Configuration.Extensions
{
    /// <summary>
    /// Extension methods for LoggerConfiguration to provide additional configuration options
    /// </summary>
    public static class LoggerConfigurationBuilderExtensions
    {

        /// <summary>
        /// Configures the logger to write log events to a file at the specified path with the given rolling interval.
        /// </summary>
        /// <remarks>This method enables file-based logging with support for rolling log files based on
        /// the specified interval. Use this to persist logs to disk and manage log file size and retention.</remarks>
        /// <param name="loggerConfiguration">The logger configuration to apply the file sink to.</param>
        /// <param name="path">The file path where log events will be written. Cannot be null, empty, or consist only of white-space
        /// characters.</param>
        /// <param name="rollingInterval">The interval at which the log file will roll over to a new file.</param>
        /// <returns>The logger configuration, allowing further configuration to be chained.</returns>
        public static LoggerConfiguration WriteToFile(this LoggerConfiguration loggerConfiguration, string path, RollingInterval rollingInterval)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));
            return loggerConfiguration.WriteTo.File(path, rollingInterval: rollingInterval);
        }

    }
}
