using Newtonsoft.Json;

namespace L1.Features.PartnerCommunicators.Award
{
    public class PartnerAwardResult : PartnerResult
    {
        [JsonConverter(typeof(OperatorPlayerIdJsonConverter))]
        public dynamic PlayerId { get; set; }
    }
}
