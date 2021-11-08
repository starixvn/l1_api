using System;
using System.Net.Http;
using System.Threading.Tasks;
using L1.Features.AdminApiCommunicators;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Maintenance;
using L1.Shared.Constants;
using Sentry;

namespace L1.Features.OWCommunicators.CheckMaintenance
{
    public interface IOWMaintenanceService
    {
        Task ObserveMaintenanceSchedule(OWRequest request);
    }

    public class OWMaintenanceService : PartnerMaintenanceService, IOWMaintenanceService
    {
        private readonly OWServiceSettings owServiceSettings;

        public OWMaintenanceService(
            HttpClient httpClient,
            OWServiceSettings owServiceSettings,
            IAdminApiService adminApiService,
            ISentryClient sentryClient)
            : base(httpClient, adminApiService, sentryClient)
        {
            this.owServiceSettings = owServiceSettings;
        }

        public Task ObserveMaintenanceSchedule(OWRequest request)
        => ObserveMaintenanceSchedule(
                PartnerIds.OW,
                new PartnerRequest(request.Call),
                new Uri(owServiceSettings.BaseUrl, owServiceSettings.Endpoints.CheckMaintenance));
    }
}