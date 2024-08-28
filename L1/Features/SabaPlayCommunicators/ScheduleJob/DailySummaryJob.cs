using System.Threading.Tasks;
using Coravel.Invocable;
using L1.Features.SabaPlayCommunicators.DailySummary;

namespace L1.Features.SabaPlayCommunicators.ScheduleJob
{
    public class DailySummaryJob : IInvocable
    {
        private readonly IDailySummaryDataAccess dailySummaryDataAccess;
        private readonly ISabaPlayDailySummaryService sabaPlayDailySummaryService;

        public DailySummaryJob()
        { }

        public async Task Invoke()
        {
            var dailySummaryData = await sabaPlayDailySummaryService.Post();

            if (dailySummaryData.IsSuccessful())
            {
                await dailySummaryDataAccess.Update(dailySummaryData.Payload);
            }
        }
    }
}