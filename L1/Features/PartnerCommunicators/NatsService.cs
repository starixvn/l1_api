using System.Text.Json;
using System.Threading.Tasks;
using NATS.Client;
using NATS.Client.JetStream;

namespace L1.Features.PartnerCommunicators
{
    public interface INatsService
    {
        Task PublishAsync(string subject, object message);
    }

    public class NatsService : INatsService
    {
        private readonly IConnection _connection;
        private readonly IJetStream _jetStream;

        public NatsService(IConnection connection)
        {
            _connection = connection;
            _jetStream = _connection.CreateJetStreamContext();
        }

        public async Task PublishAsync(string subject, object message)
        {
            var jsonMessage = JsonSerializer.Serialize(message);
            var msg = new Msg(subject, (MsgHeader)null, System.Text.Encoding.UTF8.GetBytes(jsonMessage));

            await _jetStream.PublishAsync(msg);
        }
    }
}