using System.Linq;
using System.Threading.Tasks;
using Coravel.Invocable;
using L1.Features.SabaPlayCommunicators.DailySummary;

namespace L1.Features.SabaPlayCommunicators.ScheduleJob
{
    public class DailySummaryJob : IInvocable
    {
        private readonly IDailySummaryDataAccess dailySummaryDataAccess;
        private readonly ISabaPlayDailySummaryService sabaPlayDailySummaryService;

        public DailySummaryJob(IDailySummaryDataAccess dailySummaryDataAccess, ISabaPlayDailySummaryService sabaPlayDailySummaryService)
        {
            this.dailySummaryDataAccess = dailySummaryDataAccess;
            this.sabaPlayDailySummaryService = sabaPlayDailySummaryService;
        }

        public async Task Invoke()
        {
            Sentry.SentrySdk.CaptureMessage($"Invoke DailySummaryJob!");
            var dailySummaryData = await sabaPlayDailySummaryService.Post();
            Sentry.SentrySdk.CaptureMessage($"dailySummaryData - {dailySummaryData.IsSuccessful()} - {dailySummaryData.Payload.TargetDate} - {dailySummaryData.Payload.SiteData.Count()}");

            if (dailySummaryData.IsSuccessful())
            {
                await dailySummaryDataAccess.Update(dailySummaryData.Payload);
            }
        }
    }
}