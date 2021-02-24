using L1.Features.Sites;
using L1.IdentityServerApi.IdentityStore;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace L1.IdentityServerApi.Extensions
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddMongoDb(this IIdentityServerBuilder builder, string connectionString)
        {
            var camelIgnoringExtraElementConventions = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true)
            };

            ConventionRegistry.Register(nameof(camelIgnoringExtraElementConventions), camelIgnoringExtraElementConventions, t => true);

            var client = new MongoClient(connectionString);

            builder.Services.AddSingleton<IMongoClient>(client);
            builder.Services.AddSingleton<ISiteDataAccess, SiteDataAccess>();
            builder.Services.AddSingleton<ISiteDataService, SiteDataService>();

            return builder;
        }

        public static IIdentityServerBuilder AddIdentityClients(this IIdentityServerBuilder builder) =>
            builder.AddClientStoreCache<ClientStore>();

        public static IIdentityServerBuilder AddIdentityApiResources(this IIdentityServerBuilder builder) =>
            builder.AddResourceStoreCache<ResourceStore>();
    }
}