using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace L1.Features.Sites
{
    public class SigningCredentialKey
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string PrivateKey { get; set; }

        public bool IsMain { get; set; }

        public string KeyId { get; set; }
    }
}