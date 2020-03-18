namespace L1.Features.Sites
{
    using System.Collections.Generic;

    public interface ISiteDataService
    {
        IEnumerable<Site> GetAll();

        Site GetByClientId(string id);

        Site GetBySiteId(string id);
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
    }
}