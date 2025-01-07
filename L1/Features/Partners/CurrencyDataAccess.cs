using System.Threading.Tasks;
using L1.Shared.Configurations;
using L1.Shared.Constants;
using MongoDB.Driver;

namespace L1.Features.Partners
{
    public interface ICurrencyDataAccess
    {
        Task<bool> Verify(string partnerId, string currencyName);

        Task<Currency> Get(string partnerId, string currencyName);
    }

    public class CurrencyDataAccess : ICurrencyDataAccess
    {
        private const string collectionName = "currencies";
        private readonly Collation defaultCollation = new Collation("en", strength: CollationStrength.Secondary);

        private readonly IMongoCollection<Currency> currencyCollection;

        public CurrencyDataAccess(IMongoClient mongoClient, IAppSettings appSettings)
        {
            var database = mongoClient.GetDatabase(MongoDatabases.GetMinigameDatabase(appSettings.MongoDBSettings.PrefixDbName));
            currencyCollection = database.GetCollection<Currency>(collectionName);
        }

        public async Task<bool> Verify(string partnerId, string currencyName)
        => (await currencyCollection.CountDocumentsAsync(
                currency => currency.PartnerId == partnerId &&
                            currency.Name == currencyName &&
                            currency.Enabled,
                new CountOptions
                {
                    Collation = defaultCollation
                })) > 0;

        public async Task<Currency> Get(string partnerId, string currencyName)
        => (await currencyCollection.FindAsync(
                currency => currency.PartnerId == partnerId &&
                            currency.Name == currencyName &&
                            currency.Enabled,
                new FindOptions<Currency, Currency>
                {
                    Collation = defaultCollation
                })).FirstOrDefault();
    }
}