using OW.ApiSimulator.Models.Saba;

namespace OW.ApiSimulator.Models
{
    public class DailySummaryViewModel
    {
        public DailySummaryRequest Request { get; set; } = new DailySummaryRequest();

        public SabaResponse<DailySummaryResult> Result { get; set; }
    }
}