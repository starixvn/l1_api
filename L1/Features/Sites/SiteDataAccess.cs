using System.Collections.Generic;
using System.Linq;
using L1.Shared.Constants;
using MongoDB.Driver;

namespace L1.Features.Sites
{
    public interface ISiteDataAccess
    {
        IEnumerable<Site> GetAll();

        Site GetByClientId(string id, bool isMobi);

        Site GetBySiteId(string id);

        bool VerifyClient(string clientId, string clientSecret);
    }

    public class SiteDataAccess : ISiteDataAccess
    {
        private const string collectionName = "sites";
        private readonly IMongoCollection<Site> siteCollection;

        public SiteDataAccess(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(MongoDatabases.Member);
            siteCollection = database.GetCollection<Site>(collectionName);
        }

        public IEnumerable<Site> GetAll() =>
            siteCollection.Find(site => true).ToList();

        public Site GetByClientId(string id, bool isMobi) =>
            siteCollection.Find(site => site.ClientId == id && site.IsMobi == isMobi).FirstOrDefault();

        public Site GetBySiteId(string id) =>
            siteCollection.Find(site => site.SiteId == id).FirstOrDefault();

        public bool VerifyClient(string clientId, string clientSecret)
            => siteCollection.Find(site => site.ClientId == clientId && site.ClientSecret == clientSecret).Any();
    }
}