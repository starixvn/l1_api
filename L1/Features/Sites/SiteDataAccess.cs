using System.Collections.Generic;
using System.Linq;
using L1.Shared.Constants;
using MongoDB.Driver;

namespace L1.Features.Sites
{
    public interface ISiteDataAccess
    {
        IEnumerable<Site> GetAll();

        Site GetByClientId(string id);

        Site GetBySiteId(string id);

        bool VerifyClient(string clientId, string clientSecret);

        bool VerifyClient(string clientId, string clientSecret, string currency);
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

        public Site GetByClientId(string id) =>
            siteCollection.Find(site => site.ClientId == id).FirstOrDefault();

        public Site GetBySiteId(string id) =>
            siteCollection.Find(site => site.SiteId == id).FirstOrDefault();

        public bool VerifyClient(string clientId, string clientSecret)
            => siteCollection.Find(site => site.ClientId == clientId && site.ClientSecret == clientSecret).Any();

        public bool VerifyClient(string clientId, string clientSecret, string currency)
        {
            var filter = Builders<Site>.Filter.And(
                Builders<Site>.Filter.Eq(site => site.ClientId, clientId),
                Builders<Site>.Filter.Eq(site => site.ClientSecret, clientSecret),
                Builders<Site>.Filter.AnyEq(site => site.ValidCurrencies, currency.ToUpperInvariant()));

            return siteCollection.Find(filter).Any();
        }
    }
}