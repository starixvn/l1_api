using IdentityServer4.Stores;
using L1.Features.SigningCredentials;
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
            builder.Services.AddScoped<ISiteDataAccess, SiteDataAccess>();
            builder.Services.AddScoped<ISiteDataService, SiteDataService>();
            builder.Services.AddScoped<ISigningCredentialsDataAccess, SigningCredentialsDataAccess>();

            return builder;
        }

        public static IIdentityServerBuilder AddIdentityClients(this IIdentityServerBuilder builder) =>
            builder.AddClientStoreCache<ClientStore>();

        public static IIdentityServerBuilder AddIdentityApiResources(this IIdentityServerBuilder builder) =>
            builder.AddResourceStoreCache<ResourceStore>();

        public static IIdentityServerBuilder AddSigningCredentialStore(this IIdentityServerBuilder builder)
        {
            builder.Services.AddScoped<ISigningCredentialStore, SigningCredentialStore>();
            builder.Services.AddScoped<IValidationKeysStore, SigningCredentialStore>();

            return builder;
        }
    }
}