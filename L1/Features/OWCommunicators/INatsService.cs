using System.Threading.Tasks;

namespace L1.Features.OWCommunicators
{
    public interface INatsService
    {
        Task PublishAsync(string subject, object message);
    }
}