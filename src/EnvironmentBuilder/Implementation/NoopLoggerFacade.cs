using EnvironmentBuilder.Abstractions;

namespace EnvironmentBuilder.Implementation
{
    internal class NoopLoggerFacade : ILoggerFacade
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Off;
        public void Log(LogLevel level, object message)
        {
           
        }
    }
}