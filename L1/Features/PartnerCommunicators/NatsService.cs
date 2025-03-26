using System.Threading.Tasks;
using L1.Features.Json;
using L1.Shared.Configurations;
using NATS.Client;
using NATS.Client.JetStream;
using Sentry;

namespace L1.Features.PartnerCommunicators
{
    public interface INatsService
    {
        Task PublishAsync(string subject, object message);
    }

    public class NatsService : INatsService
    {
        private readonly IJetStream jetStream;
        private readonly string prefix;
        private readonly bool enable;
        private readonly ISentryClient sentryClient;
        private readonly IJsonService jsonService;

        public NatsService(
            IConnection connection,
            IAppSettings appSettings,
            ISentryClient sentryClient,
            IJsonService jsonService)
        {
            this.jetStream = connection.CreateJetStreamContext();
            this.prefix = appSettings.StreamSettings.Prefix;
            this.enable = appSettings.StreamSettings.Enable;
            this.sentryClient = sentryClient;
            this.jsonService = jsonService;
        }

        public async Task PublishAsync(string subject, object message)
        {
            if (!enable) return;

            var formattedSubject = SubjectFormatter.FormatSubject(prefix, subject);
            var jsonMessage = jsonService.Serialize(message);
            var msg = new Msg(formattedSubject, (MsgHeader)null, System.Text.Encoding.UTF8.GetBytes(jsonMessage));

            try
            {
                await jetStream.PublishAsync(msg);
            }
            catch (NATSTimeoutException ex)
            {
                sentryClient.CaptureException(ex);
            }
        }
    }
}