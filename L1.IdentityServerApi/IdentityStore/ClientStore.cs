using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using L1.Features.Sites;

namespace L1.IdentityServerApi.IdentityStore
{
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
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret(site.ClientSecret.Sha256())
                },
                AllowedScopes = { site.SiteId }
            });
        }
    }
}