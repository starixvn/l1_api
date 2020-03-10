using System;

namespace L1.Client
{
    public class AuthenticationInfo
    {
        public AuthenticationInfo(Uri apiUri, string clientId, string clientSecret, string scope = "SingleSignOn")
        {
            ApiUri = apiUri;
            ClientId = clientId;
            ClientSecret = clientSecret;
            Scope = scope;
        }

        public Uri ApiUri { get; }

        public string ClientId { get; }

        public string ClientSecret { get; }

        public string Scope { get; }
    }
}