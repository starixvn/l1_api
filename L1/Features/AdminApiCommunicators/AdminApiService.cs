using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using L1.Features.AdminApiCommunicators.NotifyHighStake;
using L1.Features.AdminApiCommunicators.UnderMaintenance;
using L1.Features.AdminApiCommunicators.VoidTicket;
using Newtonsoft.Json;
using Sentry;
using Sentry.Protocol;

namespace L1.Features.AdminApiCommunicators
{
    public interface IAdminApiService
    {
        Task UnderMaintenance(MaintenanceRequest request);

        Task NotifyHighStake(NotifyHighStakeRequest request);

        Task VoidTicket(VoidTicketRequest request);
    }

    public class AdminApiService : IAdminApiService
    {
        private readonly HttpClient httpClient;
        private readonly AdminApiEndPoints adminApiEndPoints;
        private readonly ISentryClient sentryClient;

        public AdminApiService(HttpClient httpClient, AdminApiEndPoints adminApiEndPoints, ISentryClient sentryClient)
        {
            this.httpClient = httpClient;
            this.adminApiEndPoints = adminApiEndPoints;
            this.sentryClient = sentryClient;
        }

        public Task UnderMaintenance(MaintenanceRequest request)
            => PostAndForget(adminApiEndPoints.UnderMaintenance, request);

        public Task NotifyHighStake(NotifyHighStakeRequest request)
            => PostAndForget(adminApiEndPoints.NotifyHighStake, request);

        public Task VoidTicket(VoidTicketRequest request)
            => PostAndForget(adminApiEndPoints.VoidTicket, request);

        public async Task PostAndForget(string apiEndpoint, AdminApiRequest request)
        {
            try
            {
                await Post(apiEndpoint, request);
            }
            catch (Exception ex)
            {
                sentryClient.CaptureMessage(ex.ToString(), SentryLevel.Error);
            }
        }

        private Task<HttpResponseMessage> Post(string apiEndpoint, AdminApiRequest request)
        {
            const string defaultMediaType = "application/json";

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, defaultMediaType);

            return httpClient.PostAsync(new Uri(apiEndpoint, UriKind.Relative), content);
        }
    }
}