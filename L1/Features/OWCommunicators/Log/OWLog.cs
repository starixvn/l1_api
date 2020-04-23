using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace L1.Features.OWCommunicators.Log
{
    public class OWLog
    {
        public OWLog(DateTimeOffset requestTimeStamp, IOWCall call, DateTimeOffset responseTimeStamp, IOWResult result)
        {
            RequestTimeStamp = requestTimeStamp;
            Call = call;
            ResponseTimeStamp = responseTimeStamp;
            Result = result;
            LogTimeStamp = DateTimeOffset.UtcNow;
        }

        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset RequestTimeStamp { get; }

        public IOWCall Call { get; }

        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset ResponseTimeStamp { get; }

        public IOWResult Result { get; }

        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset LogTimeStamp { get; }
    }
}