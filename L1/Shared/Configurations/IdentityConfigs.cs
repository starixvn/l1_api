using System.Collections.Generic;
using System.Linq;
using IdentityServer4.Models;

namespace L1.Shared.Configurations
{
    public static class IdentityConfigs
    {
        public static IEnumerable<ApiResource> Apis(IEnumerable<LicenseeConfig> licenseeConfigs)
            => licenseeConfigs.Select(resource => new ApiResource(resource.SiteName, resource.SiteName));

        public static IEnumerable<Client> Clients(IEnumerable<LicenseeConfig> licenseeConfig)
            => licenseeConfig.Select(resource 
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
                    AllowedScopes = { resource.SiteName }
                });
        }
}