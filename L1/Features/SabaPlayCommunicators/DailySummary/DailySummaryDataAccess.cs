using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using L1.Shared.Configurations;
using L1.Shared.Constants;
using MongoDB.Driver;

namespace L1.Features.SabaPlayCommunicators.DailySummary
{
    public interface IDailySummaryDataAccess
    {
        Task Update(DailySummaryPayload data);
    }

    public class DailySummaryDataAccess : IDailySummaryDataAccess
    {
        private readonly IMongoCollection<OperatorDailySummary> summaryCollection;
        private readonly IAppSettings appSettings;

        public DailySummaryDataAccess(IMongoClient mongoClient, IAppSettings appSettings)
        {
            this.appSettings = appSettings;
            var database = mongoClient.GetDatabase(MongoDatabases.GetMinigameDatabase(appSettings.MongoDBSettings.PrefixDbName));

            summaryCollection = database.GetCollection<OperatorDailySummary>("operator_daily_summaries");
        }

        public async Task Update(DailySummaryPayload data)
        {
            var writes = new List<WriteModel<OperatorDailySummary>>();
            foreach (var siteData in data.SiteData)
            {
                foreach (var betData in siteData.BettingData)
                {
                    var updateDefinition = Builders<OperatorDailySummary>.Update
                        .Set(x => x.BetCount, betData.BetCount)
                        .Set(x => x.ActualStake, betData.ActualStake)
                        .Set(x => x.WinLose, betData.WinLose);

                    var filterBuilder = Builders<OperatorDailySummary>.Filter;
                    var filterDefinition = filterBuilder.And(
                        filterBuilder.Eq(x => x.OperatorId, appSettings.PartnerIds.SabaPlay),
                        filterBuilder.Eq(x => x.TargetDate, data.TargetDate),
                        filterBuilder.Eq(x => x.SiteId, siteData.SiteId),
                        filterBuilder.Eq(x => x.CurrencyIso, betData.CurrencyIso));

                    writes.Add(new UpdateOneModel<OperatorDailySummary>(filterDefinition, updateDefinition) { IsUpsert = true });

                }
            }

            if(writes.Any())
            {
                await summaryCollection.BulkWriteAsync(writes, new() { IsOrdered = false });
            }
        }
    }
}