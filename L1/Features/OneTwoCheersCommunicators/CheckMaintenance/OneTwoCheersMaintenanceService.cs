namespace L1.Features.OneTwoCheersCommunicators.CheckMaintenance
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using L1.Features.AdminApiCommunicators;
    using L1.Features.PartnerCommunicators;
    using L1.Features.PartnerCommunicators.Maintenance;
    using L1.Shared.Configurations;
    using L1.Shared.Json;
    using Sentry;

    public interface IOneTwoCheersMaintenanceService
    {
        Task ObserveMaintenanceSchedule(OneTwoCheersRequest request, string operatorId);
    }
    public class OneTwoCheersMaintenanceService : PartnerMaintenanceService, IOneTwoCheersMaintenanceService
    {
        private readonly OneTwoCheersServiceSettings oneTwoCheersServiceSettings;
        private readonly IAppSettings appSettings;

        public OneTwoCheersMaintenanceService(
            HttpClient httpClient,
            OneTwoCheersServiceSettings OneTwoCheersServiceSettings,
            IAdminApiService adminApiService,
            ISentryClient sentryClient,
            IAppSettings appSettings,
            IJsonService jsonService)
            : base(httpClient, adminApiService, sentryClient, jsonService)
        {
            this.oneTwoCheersServiceSettings = OneTwoCheersServiceSettings;
            this.appSettings = appSettings;
        }

        public Task ObserveMaintenanceSchedule(OneTwoCheersRequest request, string operatorId)
        => ObserveMaintenanceSchedule(
                operatorId,
                new PartnerRequest(request.Call),
                new Uri(oneTwoCheersServiceSettings.BaseUrl, oneTwoCheersServiceSettings.Endpoints.CheckMaintenance));
    }

}
