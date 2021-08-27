using System;
using L1.Shared.Constants;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace L1.Features.OWCommunicators.Log
{
    public class OWLog
    {
        public OWLog(DateTimeOffset requestTimeStamp, IOWCall call, DateTimeOffset responseTimeStamp, IOWResult result, byte system = AppTypes.SabaClub)
        {
            LogTimeStamp = DateTimeOffset.UtcNow;
            RequestTimeStamp = requestTimeStamp;
            Call = call;
            ResponseTimeStamp = responseTimeStamp;
            Result = result;
            System = system;
        }

        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset LogTimeStamp { get; set; }

        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset RequestTimeStamp { get; set; }

        public IOWCall Call { get; set; }

        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset ResponseTimeStamp { get; set; }

        public IOWResult Result { get; set; }

        public byte System { get; set; }
    }
}