using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using L1.Features.Sites;

namespace L1.IdentityServerApi.IdentityStore
{
    public class ResourceStore(ISiteDataService siteDataService) : IResourceStore
    {
        public Task<ApiResource> FindApiResourcesByNameAsync(string name)
        {
            var site = siteDataService.GetBySiteId(name);

            return Task.FromResult(new ApiResource(site.SiteId, site.SiteId));
        }

        public Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            var sites = siteDataService.GetAll();
            var result = GetAllApiResources(sites).Where(x => apiResourceNames.Contains(x.Name));

            return Task.FromResult(result);
        }
        public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var sites = siteDataService.GetAll();
            var result = GetAllApiResources(sites).Where(resource => resource.Scopes.Any(scope => scopeNames.Contains(scope)));

            return Task.FromResult(result);
        }

        public Task<Resources> GetAllResourcesAsync()
        {
            var sites = siteDataService.GetAll();
            var apiResources = GetAllApiResources(sites);
            var apiScopes = Enumerable.Empty<ApiScope>();
            var result = new Resources(Enumerable.Empty<IdentityResource>(), apiResources, apiScopes);

            return Task.FromResult(result);
        }

        private static IEnumerable<ApiResource> GetAllApiResources(IEnumerable<Site> sites)
        {
            return sites.Select(site =>
            {
                var apiResource = new ApiResource(site.SiteId, site.SiteId);
                apiResource.Scopes.Add(site.SiteId);
                return apiResource;
            });
        }

        public Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            return Task.FromResult(Enumerable.Empty<ApiScope>());
        }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            return Task.FromResult(Enumerable.Empty<IdentityResource>());
        }
    }
}