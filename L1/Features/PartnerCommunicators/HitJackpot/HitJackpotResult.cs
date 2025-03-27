using System.Text.Json.Serialization;

namespace L1.Features.PartnerCommunicators.HitJackpot
{
    public class HitJackpotResult : PartnerResult
    {
        [JsonConverter(typeof(OperatorPlayerIdJsonConverter))]
        public dynamic PlayerId { get; set; }
    }
}