using System.Collections.Generic;
using L1.Shared.Constants;
using MongoDB.Driver;

namespace L1.Features.Sites
{
    public interface ISiteDataAccess
    {
        IEnumerable<Site> GetAll();

        Site GetByClientId(string id);

        Site GetBySiteId(string id);
    }

    public class SiteDataAccess : ISiteDataAccess
    {
        private const string collectionName = "sites";
        private readonly IMongoCollection<Site> siteCollection;

        public SiteDataAccess(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(MongoDatabases.Config);
            siteCollection = database.GetCollection<Site>(collectionName);
        }

        public IEnumerable<Site> GetAll() =>
            siteCollection.Find(site => true).ToList();

        public Site GetByClientId(string id) =>
            siteCollection.Find(site => site.ClientId == id).FirstOrDefault();

        public Site GetBySiteId(string id) =>
           siteCollection.Find(site => site.SiteId == id).FirstOrDefault();
    }
}