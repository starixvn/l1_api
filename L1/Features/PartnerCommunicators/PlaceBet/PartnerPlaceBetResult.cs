using System.Text.Json;
using System;
using System.Text.Json.Serialization;

namespace L1.Features.PartnerCommunicators.PlaceBet
{
    public class PartnerPlaceBetResult : PartnerResult
    {
        public long? ObTransId { get; set; }

        [JsonConverter(typeof(OperatorPlayerIdJsonConverter))]
        public dynamic PlayerId { get; set; }

        public decimal Balance { get; set; }

        public decimal MinBet { get; set; }

        public decimal MaxBet { get; set; }
    }

   
}