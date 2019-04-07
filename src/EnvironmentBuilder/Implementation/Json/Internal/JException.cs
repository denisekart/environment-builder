using System;

namespace EnvironmentBuilder.Implementation.Json
{
    public class JException : Exception
    {
        public JException()
        {
            
        }

        public JException(string message):base(message)
        {
            
        }
    }
}