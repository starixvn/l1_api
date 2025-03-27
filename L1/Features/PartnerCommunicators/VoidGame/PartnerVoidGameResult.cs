using System.Text.Json.Serialization;

namespace L1.Features.PartnerCommunicators.VoidGame
{
    public class PartnerVoidGameResult : PartnerResult
    {
        [JsonConverter(typeof(OperatorPlayerIdJsonConverter))]
        public dynamic PlayerId { get; set; }
    }
}