namespace L1.Features.Sites
{
    using System.Collections.Generic;
    using System.Linq;
    using MongoDB.Driver;

    public interface ISiteDataAccess
    {
        IEnumerable<Site> GetAll();

        Site Get(string siteId);
    }

    public class SiteDataAccess : ISiteDataAccess
    {
        private const string collectionName = "sites";
        private const string databaseName = "config";
        private readonly IMongoCollection<Site> siteCollection;

        public SiteDataAccess(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(databaseName);
            siteCollection = database.GetCollection<Site>(collectionName);
        }

        public IEnumerable<Site> GetAll() =>
            siteCollection.Find(site => true).ToList();

        public Site Get(string id) =>
            siteCollection.Find(site => site.SiteId == id).FirstOrDefault();
    }
}