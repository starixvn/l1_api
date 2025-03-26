using System.Text.Json.Serialization;

namespace L1.Features.PartnerCommunicators.Award
{
    public class PartnerAwardResult : PartnerResult
    {
        [JsonConverter(typeof(OperatorPlayerIdConverter))]
        public dynamic PlayerId { get; set; }
    }
}
