using DiscordBot.Controller;
using DiscordBot.Startup.Configuration;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace DiscordBot.Logging
{
    internal class Logger : ILogger
    {
        private readonly string _logDirectory;

        private readonly LogLevel _logLevel;

        private readonly List<string> _logs;

        public IReadOnlyList<string> Logs => _logs;

        private readonly ConsoleIOHandler _consoleIOHandler;

        public Logger(IOptions<LoggerConfiguration> options, ConsoleIOHandler consoleIOHandler)
        {
            _logDirectory = options.Value.Path;
            _logs = [];
            _consoleIOHandler = consoleIOHandler;
        }

        public void LogDebug(string message)
        {
            if (_logLevel <= LogLevel.Debug)
            {
                _logs.Add($"Debug: {message}");
            }
        }

        public void LogInfo(string message)
        {
            if (_logLevel <= LogLevel.Info)
            {
                _logs.Add($"Info: {message}");
            }
        }

        public void LogWarning(string message)
        {
            if (_logLevel <= LogLevel.Warn)
            {
                _logs.Add($"Warning: {message}");
            }
        }

        public void LogException(Exception ex)
        {
            if (_logLevel <= LogLevel.Error)
            {
                var baseException = ex.GetBaseException();
                _logs.Add($"---EXCEPTION--- {ex.StackTrace}: {baseException.Message}");
            }
        }

        public void LogException(Exception ex, string message)
        {
            if (_logLevel <= LogLevel.Error)
            {
                var baseException = ex.GetBaseException();
                _logs.Add($"---EXCEPTION--- {message}\n{ex.StackTrace}: {baseException.Message}");
            }
        }

        //TODO: Open a FileStreamWriter and write logs on the fly
        public void WriteLogsToFile()
        {
            try
            {
                if (_logs.Count > 0)
                {
                    string data = "";
                    for (int i = 0; i < _logs.Count; ++i)
                    {
                        data += _logs[i] + "\r\n\r\n";
                    }

                    string filePath = _logDirectory + DateTime.Now.ToString("yyyy-MMM-d_HH-mm-ss") + ".txt";
                    FileInfo fileInfo = new(filePath);
                    fileInfo.Directory?.Create();
                    File.WriteAllText(fileInfo.FullName, data);
                   _consoleIOHandler.PrintMessage($"Logs written to {fileInfo.FullName}");
                }

            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
        }

        public void WriteLogsToFile(string filePath)
        {
            try
            {
                if (_logs.Count > 0)
                {
                    string data = "";
                    for (int i = 0; i < _logs.Count; ++i)
                    {
                        data += _logs[i] + "\r\n\r\n";
                    }

                    FileInfo fileInfo = new(filePath);
                    fileInfo.Directory?.Create();
                    File.WriteAllText(fileInfo.FullName, data);
                    _consoleIOHandler.PrintMessage($"Logs written to {fileInfo.FullName}");
                }

            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
        }
    }
}
