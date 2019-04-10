using EnvironmentBuilder.Abstractions;

namespace EnvironmentBuilder.Implementation
{
    internal class NoopLoggerFascade : ILoggerFascade
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Off;
        public void Log(LogLevel level, object message)
        {
           
        }
    }
}