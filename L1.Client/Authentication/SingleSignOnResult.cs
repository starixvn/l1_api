using System;

namespace L1.Client
{
    public class SingleSignOnResult : Result
    {
        public SingleSignOnResult(Uri redirectUri) : base(null)
        {
            RedirectUri = redirectUri;
        }

        public SingleSignOnResult(Error error) : base(error)
        {
        }

        public Uri RedirectUri { get; }
    }
}