using System.Threading.Tasks;
using L1.Shared.Constants;
using MongoDB.Driver;

namespace L1.Features.MemberAuthentications
{
    public interface IMemberAuthDataAccess
    {
        Task<MemberAuth> GetByToken(string token);
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

        public async Task<MemberAuth> GetByToken(string token) =>
            (await memberAuthCollection.FindAsync(memberAuth => memberAuth.Token == token)).FirstOrDefault();
    }
}