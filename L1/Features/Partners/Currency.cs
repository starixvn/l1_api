using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace L1.Features.Partners
{
    public class Currency
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string PartnerId { get; set; }
       
        [BsonRepresentation(BsonType.ObjectId)]
        public string GroupId { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public float Multiplier { get; set; }

        public bool Enabled { get; set; }

        public bool IsTest { get; set; }
    }
}