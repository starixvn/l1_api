using L1.Features.AdminApiCommunicators;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Maintenance;
using L1.Shared.Configurations;
using Sentry;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace L1.Features.HotdogCommunicators.CheckMaintenance
{
    public interface IHotdogMaintenanceService
    {
        Task ObserveMaintenanceSchedule(HotdogRequest request);
    }

    public class HotdogMaintenanceService : PartnerMaintenanceService, IHotdogMaintenanceService
    {
        private readonly HotdogServiceSettings HotdogServiceSettings;
        private readonly IAppSettings appSettings;

        public HotdogMaintenanceService(
            HttpClient httpClient,
            HotdogServiceSettings HotdogServiceSettings,
            IAdminApiService adminApiService,
            ISentryClient sentryClient,
            IAppSettings appSettings)
            : base(httpClient, adminApiService, sentryClient)
        {
            this.HotdogServiceSettings = HotdogServiceSettings;
            this.appSettings = appSettings;
        }

        public Task ObserveMaintenanceSchedule(HotdogRequest request)
        => ObserveMaintenanceSchedule(
                appSettings.PartnerIds.SabaPlay_HD,
                new PartnerRequest(request.Call),
                new Uri(HotdogServiceSettings.BaseUrl, HotdogServiceSettings.Endpoints.CheckMaintenance));
    }
}