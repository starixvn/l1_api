using System;

namespace L1.Client
{
    public class AuthenticationInfo
    {
        public AuthenticationInfo(
            Uri apiUri, 
            Uri authUri,
            string clientId, 
            string clientSecret, 
            string siteName)
        {
            ApiUri = apiUri;
            AuthUri = authUri;
            ClientId = clientId;
            ClientSecret = clientSecret;
            SiteName = siteName;
        }

        public Uri ApiUri { get; }

        public Uri AuthUri { get; }

        public string ClientId { get; }

        public string ClientSecret { get; }

        public string SiteName { get; }
    }
}