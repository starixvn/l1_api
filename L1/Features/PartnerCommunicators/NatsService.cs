using System.Threading.Tasks;
using NATS.Client;
using NATS.Client.JetStream;
using L1.Shared.Configurations;
using Sentry;
using L1.Shared.Services;

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
            this.sentryClient = sentryClient;
            this.jsonService = jsonService;
        }

        public async Task PublishAsync(string subject, object message)
        {
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