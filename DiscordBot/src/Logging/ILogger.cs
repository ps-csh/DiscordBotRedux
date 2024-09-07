namespace DiscordBot.Logging
{
    public interface ILogger
    {
        public IReadOnlyList<string> Logs { get; }

        public void LogDebug(string message);

        public void LogInfo(string message);

        public void LogWarning(string message);

        public void LogException(Exception ex);

        public void LogException(Exception ex, string message);

        public void WriteLogsToFile();

        public void WriteLogsToFile(string filePath);
    }
}
