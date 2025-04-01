using Newtonsoft.Json;

namespace L1.Features.PartnerCommunicators.EndGame
{
    public class PartnerEndGameResult : PartnerResult
    {
        [JsonConverter(typeof(OperatorPlayerIdJsonConverter))]
        public dynamic PlayerId { get; set; }
    }
}