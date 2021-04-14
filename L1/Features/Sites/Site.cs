using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace L1.Features.Sites
{
    public class Site
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string SiteId { get; set; }

        public string BackendApi { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string GameClientUrl { get; set; }

        public bool IsMobi { get; set; }
    }
}