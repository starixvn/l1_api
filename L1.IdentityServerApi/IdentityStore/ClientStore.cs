namespace L1.IdentityServerApi.IdentityStore
{
    using System.Threading.Tasks;
    using IdentityServer4.Models;
    using IdentityServer4.Stores;
    using L1.Features.Sites;

    public class ClientStore : IClientStore
    {
        private readonly ISiteDataService siteDataService;

        public ClientStore(ISiteDataService siteDataService)
        {
            this.siteDataService = siteDataService;
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            var site = siteDataService.GetByClientId(clientId);

            return Task.FromResult(new Client
            {
                ClientId = site.ClientId,

                // no interactive user, use the clientid/secret for authentication
                AllowedGrantTypes = GrantTypes.ClientCredentials,

                // secret for authentication
                ClientSecrets = 
                {
                    new Secret(site.ClientSecret.Sha256())
                },

                // scopes that client has access to
                AllowedScopes = { site.SiteId }
            });
        }
    }
}
