using L1.Features.SabaPlayCommunicators;
using L1.Features.SabaPlayCommunicators.DailySummary;

namespace L1.Shared.Configurations
{
    public partial interface IAppSettings
    {
        SabaPlayServiceSettings SabaPlayServiceSettings { get; set; }
        SabaPlayDailySummaryServiceSettings SabaPlayDailySummaryServiceSettings { get; set; }
    }

    public partial class AppSettings : IAppSettings
    {
        public SabaPlayServiceSettings SabaPlayServiceSettings { get; set; }
        public SabaPlayDailySummaryServiceSettings SabaPlayDailySummaryServiceSettings { get; set; }
    }
}