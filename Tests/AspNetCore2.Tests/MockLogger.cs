using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Sustainsys.Saml2.AspNetCore2.Tests
{
    internal class MockLogger : ILogger<Saml2Handler>
    {
        public MockLogger()
        {
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        public LogLevel ReceivedLogLevel { get; set; }

        public string ReceivedMessage { get; set; }

        public Exception ReceivedException { get; set; }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            ReceivedLogLevel = logLevel;
            ReceivedMessage = state.ToString();
            ReceivedException = exception;
        }
    }
}