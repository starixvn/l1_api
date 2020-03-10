using System.Collections.Generic;
using MongoDB.Driver;

namespace L1.Features.Authentications
{
    public interface IMemberAuthDataAccess
    {
        IEnumerable<MemberAuth> Get();

        MemberAuth Get(string id);

        MemberAuth Create(MemberAuth memberAuth);

        void Update(string id, MemberAuth memberAuth);
    }

    public class MemberAuthDataAccess : IMemberAuthDataAccess
    {
        private const string collectionName = "MemberAuths";
        private readonly IMongoCollection<MemberAuth> memberAuths;

        public MemberAuthDataAccess(IMongoDatabase mongoDatabase)
        {
            memberAuths = mongoDatabase.GetCollection<MemberAuth>(collectionName);
        }

        public IEnumerable<MemberAuth> Get() =>
            memberAuths.Find(memberAuth => true).ToList();

        public MemberAuth Get(string id) =>
            memberAuths.Find(memberAuth => memberAuth.Id == id).FirstOrDefault();

        public MemberAuth Create(MemberAuth memberAuth)
        {
            memberAuths.InsertOne(memberAuth);

            return memberAuth;
        }

        public void Update(string id, MemberAuth memberAuth) =>
            memberAuths.ReplaceOne(book => book.Id == id, memberAuth);
    }
}