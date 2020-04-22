using System.Collections.Generic;
using L1.Shared.Constants;
using MongoDB.Driver;

namespace L1.Features.MemberAuthentications
{
    public interface IMemberAuthDataAccess
    {
        IEnumerable<MemberAuth> GetAll();

        MemberAuth Get(string id);

        MemberAuth GetByToken(string token);

        MemberAuth Create(MemberAuth memberAuth);

        void Update(string id, MemberAuth memberAuth);

        MemberAuth GetLatest(MemberAuth memberAuth);
    }

    public class MemberAuthDataAccess : IMemberAuthDataAccess
    {
        private const string collectionName = "member_auths";
        private readonly IMongoCollection<MemberAuth> memberAuthCollection;

        public MemberAuthDataAccess(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(MongoDatabases.Member);
            memberAuthCollection = database.GetCollection<MemberAuth>(collectionName);
        }

        public IEnumerable<MemberAuth> GetAll() =>
            memberAuthCollection.Find(memberAuth => true).ToList();

        public MemberAuth Get(string id) =>
            memberAuthCollection.Find(memberAuth => memberAuth.Id == id).FirstOrDefault();

        public MemberAuth GetLatest(MemberAuth memberAuth) =>
            memberAuthCollection
                .Find(
                    x => x.IP == memberAuth.IP
                    && x.MemberId == memberAuth.MemberId
                    && x.SiteId.ToUpper() == memberAuth.SiteId.ToUpper()
                    && x.BrowserUserAgent.ToUpper() == memberAuth.BrowserUserAgent.ToUpper())
                .SortByDescending(x => x.Time)
                .Limit(1)
                .FirstOrDefault();

        public MemberAuth GetByToken(string token) =>
            memberAuthCollection.Find(memberAuth => memberAuth.Token == token).FirstOrDefault();

        public MemberAuth Create(MemberAuth memberAuth)
        {
            memberAuthCollection.InsertOne(memberAuth);

            return memberAuth;
        }

        public void Update(string id, MemberAuth memberAuth) =>
            memberAuthCollection.ReplaceOne(memberAuth => memberAuth.Id == id, memberAuth);
    }
}