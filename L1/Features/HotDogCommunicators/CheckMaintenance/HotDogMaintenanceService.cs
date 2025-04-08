using L1.Features.AdminApiCommunicators;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Maintenance;
using L1.Shared.Json;
using Sentry;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace L1.Features.HotdogCommunicators.CheckMaintenance
{
    public interface IHotdogMaintenanceService
    {
        Task ObserveMaintenanceSchedule(HotdogRequest request, string operatorId);
    }

    public class HotdogMaintenanceService : PartnerMaintenanceService, IHotdogMaintenanceService
    {
        private readonly HotdogServiceSettings hotdogServiceSettings;
        private readonly string secretKey;
        private readonly IJsonService jsonService;


        public HotdogMaintenanceService(
            HttpClient httpClient,
            HotdogServiceSettings hotdogServiceSettings,
            IAdminApiService adminApiService,
            ISentryClient sentryClient,
            IJsonService jsonService)
            : base(httpClient, adminApiService, sentryClient, jsonService)
        {
            this.hotdogServiceSettings = hotdogServiceSettings;
            this.secretKey = hotdogServiceSettings.SecretKey;
            this.jsonService = jsonService;
        }

        public Task ObserveMaintenanceSchedule(HotdogRequest request, string operatorId)
        => ObserveMaintenanceSchedule(
                operatorId,
                new PartnerRequest(request.Call),
                new Uri(hotdogServiceSettings.BaseUrl, hotdogServiceSettings.Endpoints.CheckMaintenance));
    }
}