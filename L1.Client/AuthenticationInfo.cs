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
            string siteId)
        {
            ApiUri = apiUri;
            AuthUri = authUri;
            ClientId = clientId;
            ClientSecret = clientSecret;
            SiteId = siteId;
        }

        public Uri ApiUri { get; }

        public Uri AuthUri { get; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string SiteId { get; }
    }
}