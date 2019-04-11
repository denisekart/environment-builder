using System;
using EnvironmentBuilder.Abstractions;

namespace EnvironmentBuilder.Implementation
{
    internal class ConsoleLoggerFacade : ILoggerFacade
    {
        


        public LogLevel LogLevel { get; set; } = LogLevel.Information;
        public bool WriteExceptionsToStdError { get; set; } = false;
        public void Log(LogLevel level, object message)
        {
            var l = (int) LogLevel;
            switch (level)
            {
                case LogLevel.Fatal when l>=(int)Abstractions.LogLevel.Fatal:
                    EmitFatal(message);
                    break;
                case LogLevel.Error when l >= (int)Abstractions.LogLevel.Error:
                    EmitError(message);
                    break;
                case LogLevel.Warning when l >= (int)Abstractions.LogLevel.Warning:
                    EmitWarning(message);
                    break;
                case LogLevel.Information when l >= (int)Abstractions.LogLevel.Information:
                    EmitInformation(message);
                    break;
                case LogLevel.Debug when l >= (int)Abstractions.LogLevel.Debug:
                    EmitDebug(message);
                    break;
                case LogLevel.Trace when l >= (int)Abstractions.LogLevel.Trace:
                    EmitTrace(message);
                    break;
                case LogLevel.Off:
                default:
                    break;
            }
        }

        private void EmitFatal(object message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Red;
            if(WriteExceptionsToStdError)
                Console.Error.WriteLine($"[FATAL] {message?.ToString()}");
            else
                Console.WriteLine( $"[FATAL] {message?.ToString()}");
            Console.ResetColor();
        }
        private void EmitError(object message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if (WriteExceptionsToStdError)
                Console.Error.WriteLine($"[ERROR] {message?.ToString()}");
            else
                Console.WriteLine($"[ERROR] {message?.ToString()}");
            Console.ResetColor();
        }
        private void EmitWarning(object message)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"[WARNING] {message?.ToString()}");
            Console.ResetColor();
        }
        private void EmitInformation(object message)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine($"[INFO] {message?.ToString()}");
            Console.ResetColor();
        }
        private void EmitDebug(object message)
        {
            Console.WriteLine($"[DEBUG] {message?.ToString()}");
        }
        private void EmitTrace(object message)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"[TRACE] {message?.ToString()}");
            Console.ResetColor();
        }
    }
}
