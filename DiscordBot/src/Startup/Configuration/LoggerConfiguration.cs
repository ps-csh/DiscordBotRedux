using DiscordBot.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Startup.Configuration
{
    internal class LoggerConfiguration
    {
        /// <summary>
        /// The directory to store Log files in.
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// If the path is relative to the executable
        /// </summary>
        public bool IsRelative { get; init; }

        /// <summary>
        /// The minimum level of logs to write
        /// </summary>
        public LogLevel Level { get; init; } = LogLevel.Info;
    }
}
