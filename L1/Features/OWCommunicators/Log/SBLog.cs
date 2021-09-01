using System;
using L1.Shared.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace L1.Features.OWCommunicators.Log
{
    public class SBLog
    {
        public SBLog(IRequest request, IResult result, DateTimeOffset requestTimeStamp, DateTimeOffset responseTimeStamp)
        {
            LogTimeStamp = DateTimeOffset.UtcNow;
            RequestTimeStamp = requestTimeStamp;
            Request = request;
            ResponseTimeStamp = responseTimeStamp;
            Result = result;
        }

        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset LogTimeStamp { get; set; }

        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset RequestTimeStamp { get; set; }

        public IRequest Request { get; set; }

        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset ResponseTimeStamp { get; set; }

        public IResult Result { get; set; }
    }
}