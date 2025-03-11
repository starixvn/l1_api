using L1.Features.AdminApiCommunicators;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Maintenance;
using L1.Shared.Configurations;
using Sentry;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using L1.Shared.Services;

namespace L1.Features.SabaPlayCommunicators.CheckMaintenance
{
    public interface ISabaPlayMaintenanceService
    {
        Task ObserveMaintenanceSchedule(SabaPlayRequest request);
    }

    public class SabaPlayMaintenanceService : PartnerMaintenanceService, ISabaPlayMaintenanceService
    {
        private readonly SabaPlayServiceSettings SabaPlayServiceSettings;
        private readonly IAppSettings appSettings;

        public SabaPlayMaintenanceService(
            HttpClient httpClient,
            SabaPlayServiceSettings SabaPlayServiceSettings,
            IAdminApiService adminApiService,
            ISentryClient sentryClient,
            IAppSettings appSettings,
            IJsonService jsonService)
            : base(httpClient, adminApiService, sentryClient, jsonService)
        {
            this.SabaPlayServiceSettings = SabaPlayServiceSettings;
            this.appSettings = appSettings;
        }

        public Task ObserveMaintenanceSchedule(SabaPlayRequest request)
        => ObserveMaintenanceSchedule(
                appSettings.PartnerIds.SabaPlay,
                new PartnerRequest(request.Call),
                new Uri(SabaPlayServiceSettings.BaseUrl, SabaPlayServiceSettings.Endpoints.CheckMaintenance));
    }
}