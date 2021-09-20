using System;
using System.Threading.Tasks;
using Sentry;
using Sentry.Protocol;

namespace L1.Tests._Shared
{
    public class MockSentryClient : ISentryClient
    {
        public bool IsEnabled { get; }

        public SentryId CaptureEvent(SentryEvent evt, Scope scope = null)
        {
            return SentryId.Empty;
        }

        public Task FlushAsync(TimeSpan timeout)
        {
            return null;
        }
    }
}
