using OW.ApiSimulator.Models.Saba;

namespace OW.ApiSimulator.Models
{
    public class RetriveEndGameInfoViewModel
    {
        public EndGameInfoRequest Request { get; set; } = new EndGameInfoRequest();

        public SabaResponse<EndGameInfoResult> Result { get; set; }
    }
}