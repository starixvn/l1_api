using System.Collections.Generic;

namespace L1.Features.Sites
{
    public interface ISiteDataService
    {
        IEnumerable<Site> GetAll();

        Site GetByClientId(string id);

        Site GetBySiteId(string id);

        bool VerifyClient(string clientId, string clientSecret);

        bool VerifyClient(string clientId, string clientSecret, string currency);
    }

    public class SiteDataService : ISiteDataService
    {
        private readonly ISiteDataAccess siteDataAccess;

        public SiteDataService(ISiteDataAccess siteDataAccess)
        {
            this.siteDataAccess = siteDataAccess;
        }

        public IEnumerable<Site> GetAll() => siteDataAccess.GetAll();

        public Site GetByClientId(string id) => siteDataAccess.GetByClientId(id);

        public Site GetBySiteId(string id) => siteDataAccess.GetBySiteId(id);

        public bool VerifyClient(string clientId, string clientSecret)
            => siteDataAccess.VerifyClient(clientId, clientSecret);

        public bool VerifyClient(string clientId, string clientSecret, string currency)
            => siteDataAccess.VerifyClient(clientId, clientSecret, currency);
    }
}