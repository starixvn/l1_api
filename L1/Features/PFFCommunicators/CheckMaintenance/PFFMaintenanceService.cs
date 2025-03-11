using L1.Features.AdminApiCommunicators;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Maintenance;
using L1.Shared.Configurations;
using Sentry;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using L1.Shared.Services;

namespace L1.Features.PFFCommunicators.CheckMaintenance
{
    public interface IPFFMaintenanceService
    {
        Task ObserveMaintenanceSchedule(PFFRequest request);
    }

    public class PFFMaintenanceService : PartnerMaintenanceService, IPFFMaintenanceService
    {
        private readonly PFFServiceSettings PFFServiceSettings;
        private readonly IAppSettings appSettings;
        private readonly IJsonService jsonService;

        public PFFMaintenanceService(
            HttpClient httpClient,
            PFFServiceSettings PFFServiceSettings,
            IAdminApiService adminApiService,
            ISentryClient sentryClient,
            IAppSettings appSettings,
            IJsonService jsonService)
            : base(httpClient, adminApiService, sentryClient, jsonService)
        {
            this.PFFServiceSettings = PFFServiceSettings;
            this.appSettings = appSettings;
            this.jsonService = jsonService;
        }

        public Task ObserveMaintenanceSchedule(PFFRequest request)
        => ObserveMaintenanceSchedule(
                appSettings.PartnerIds.PFF,
                new PartnerRequest(request.Call),
                new Uri(PFFServiceSettings.BaseUrl, PFFServiceSettings.Endpoints.CheckMaintenance));
    }
}