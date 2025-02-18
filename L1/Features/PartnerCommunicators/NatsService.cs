using System.Text.Json;
using System.Threading.Tasks;
using NATS.Client;
using NATS.Client.JetStream;
using L1.Shared.Configurations;

namespace L1.Features.PartnerCommunicators
{
    public interface INatsService
    {
        Task PublishAsync(string subject, object message);
    }

    public class NatsService : INatsService
    {
        private readonly IConnection connection;
        private readonly IJetStream jetStream;
        private readonly string prefix;

        public NatsService(IConnection connection, IAppSettings appSettings)
        {
            this.connection = connection;
            jetStream = this.connection.CreateJetStreamContext();
            prefix = appSettings.StreamSettings.Prefix;
        }

        public async Task PublishAsync(string subject, object message)
        {
            var formattedSubject = SubjectFormatter.FormatSubject(prefix, subject);
            var jsonMessage = JsonSerializer.Serialize(message);
            var msg = new Msg(formattedSubject, (MsgHeader)null, System.Text.Encoding.UTF8.GetBytes(jsonMessage));

            await jetStream.PublishAsync(msg);
        }
    }
}