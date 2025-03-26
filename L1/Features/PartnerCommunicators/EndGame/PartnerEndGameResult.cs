using System.Text.Json.Serialization;

namespace L1.Features.PartnerCommunicators.EndGame
{
    public class PartnerEndGameResult : PartnerResult
    {
        [JsonConverter(typeof(OperatorPlayerIdConverter))]
        public dynamic PlayerId { get; set; }
    }
}