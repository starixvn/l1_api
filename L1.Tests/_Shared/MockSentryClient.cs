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

        public SentryId CaptureEvent(SentryEvent evt, Hint hint, Scope scope = null)
        {
            return SentryId.Empty;
        }

        public void CaptureSession(SessionUpdate sessionUpdate)
        {
            throw new NotImplementedException();
        }

        public void CaptureTransaction(Transaction transaction)
        {
            throw new NotImplementedException();
        }

        public void CaptureTransaction(Transaction transaction, Hint hint)
        {
            throw new NotImplementedException();
        }

        public void CaptureUserFeedback(UserFeedback userFeedback)
        {
            throw new NotImplementedException();
        }

        public Task FlushAsync(TimeSpan timeout)
        {
            return null;
        }
    }
}
