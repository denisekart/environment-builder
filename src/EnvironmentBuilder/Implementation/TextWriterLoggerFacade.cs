using System;
using System.IO;
using EnvironmentBuilder.Abstractions;

namespace EnvironmentBuilder.Implementation
{
    internal class TextWriterLoggerFacade : ILoggerFacade
    {
        internal TextWriter Writer { get; }

        public TextWriterLoggerFacade(TextWriter writer)
        {
            Writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }


        public LogLevel LogLevel { get; set; } = LogLevel.Information;
        public void Log(LogLevel level, object message)
        {
            var l = (int)LogLevel;
            switch (level)
            {
                case LogLevel.Fatal when l >= (int)Abstractions.LogLevel.Fatal:
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
            Writer.WriteLine($"[FATAL] {message?.ToString()}");
        }
        private void EmitError(object message)
        {
            Writer.WriteLine($"[ERROR] {message?.ToString()}");
        }
        private void EmitWarning(object message)
        {
            Writer.WriteLine($"[WARNING] {message?.ToString()}");
        }
        private void EmitInformation(object message)
        {
            Writer.WriteLine($"[INFO] {message?.ToString()}");
        }
        private void EmitDebug(object message)
        {
            Writer.WriteLine($"[DEBUG] {message?.ToString()}");
        }
        private void EmitTrace(object message)
        {
            Writer.WriteLine($"[TRACE] {message?.ToString()}");
        }
    }
}