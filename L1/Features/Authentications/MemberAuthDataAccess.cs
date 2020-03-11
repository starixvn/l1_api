using System.Collections.Generic;
using MongoDB.Driver;

namespace L1.Features.Authentications
{
    public interface IMemberAuthDataAccess
    {
        IEnumerable<MemberAuth> GetAll();

        MemberAuth Get(string id);

        MemberAuth GetByToken(string token);

        MemberAuth Create(MemberAuth memberAuth);

        void Update(string id, MemberAuth memberAuth);
    }

    public class MemberAuthDataAccess : IMemberAuthDataAccess
    {
        private const string collectionName = "MemberAuths";
        private readonly IMongoCollection<MemberAuth> memberAuthCollection;

        public MemberAuthDataAccess(IMongoDatabase mongoDatabase)
        {
            memberAuthCollection = mongoDatabase.GetCollection<MemberAuth>(collectionName);
        }

        public IEnumerable<MemberAuth> GetAll() =>
            memberAuthCollection.Find(memberAuth => true).ToList();

        public MemberAuth Get(string id) =>
            memberAuthCollection.Find(memberAuth => memberAuth.Id == id).FirstOrDefault();

        public MemberAuth GetByToken(string token) =>
            memberAuthCollection.Find(memberAuth => memberAuth.Token == token).FirstOrDefault();

        public MemberAuth Create(MemberAuth memberAuth)
        {
            memberAuthCollection.InsertOne(memberAuth);
            var returnedData = GetByToken(memberAuth.Token);
            return memberAuth;
        }

        public void Update(string id, MemberAuth memberAuth) =>
            memberAuthCollection.ReplaceOne(book => book.Id == id, memberAuth);
    }
}