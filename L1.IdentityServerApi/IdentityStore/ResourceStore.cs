using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using L1.Features.Sites;

namespace L1.IdentityServerApi.IdentityStore
{
    public class ResourceStore : IResourceStore
    {
        private readonly ISiteDataService siteDataService;

        public ResourceStore(ISiteDataService siteDataService)
        {
            this.siteDataService = siteDataService;
        }

        public Task<ApiResource> FindApiResourceAsync(string name)
        {
            var site = siteDataService.GetBySiteId(name);

            return Task.FromResult(new ApiResource(site.SiteId, site.SiteId));
        }

        public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var sites = siteDataService.GetAll();
            var result = GetAllApiResources(sites).Where(x => x.Scopes.Any(s => scopeNames.Contains(s.Name)));

            return Task.FromResult(result);
        }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            return Task.FromResult(Enumerable.Empty<IdentityResource>());
        }

        public Task<Resources> GetAllResourcesAsync()
        {
            var sites = siteDataService.GetAll();
            var result = new Resources(Enumerable.Empty<IdentityResource>(), GetAllApiResources(sites));

            return Task.FromResult(result);
        }

        private IEnumerable<ApiResource> GetAllApiResources(IEnumerable<Site> sites)
        {
            return sites.Select(site => new ApiResource(site.SiteId, site.SiteId));
        }
    }
}