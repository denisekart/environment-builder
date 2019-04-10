using System;
using System.Collections.Generic;
using System.Text;

namespace EnvironmentBuilder.Abstractions
{
    public interface ILoggerFascade
    {
        LogLevel LogLevel { get; set; }
        void Log(LogLevel level, object message);
    }
}
