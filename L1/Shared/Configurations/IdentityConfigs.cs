using System.Collections.Generic;
using System.Linq;
using IdentityServer4.Models;
using L1.Features.Sites;

namespace L1.Shared.Configurations
{
    public static class IdentityConfigs
    {
        public static IEnumerable<ApiResource> Apis(IEnumerable<Site> sites)
            => sites.Select(resource => new ApiResource(resource.SiteId, resource.SiteId));

        public static IEnumerable<Client> Clients(IEnumerable<Site> sites)
            => sites.Select(resource
                => new Client
                {
                    ClientId = resource.ClientId,

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret(resource.ClientSecret.Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes = { resource.SiteId }
                });
    }
}