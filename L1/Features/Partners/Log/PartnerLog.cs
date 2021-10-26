using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace L1.Features.Partners.Log
{
    public class PartnerLog
    {
        public PartnerLog(
            DateTimeOffset requestTimeStamp,
            IPartnerCall call,
            DateTimeOffset responseTimeStamp,
            IPartnerResult result,
            byte system,
            string accessKey = "")
        {
            LogTimeStamp = DateTimeOffset.UtcNow;
            RequestTimeStamp = requestTimeStamp;
            Call = call;
            ResponseTimeStamp = responseTimeStamp;
            Result = result;
            System = system;
            AccessKey = accessKey;
        }

        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset LogTimeStamp { get; set; }

        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset RequestTimeStamp { get; set; }

        public IPartnerCall Call { get; set; }

        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset ResponseTimeStamp { get; set; }

        public IPartnerResult Result { get; set; }

        public byte System { get; set; }

        public string AccessKey { get; set; }
    }
}