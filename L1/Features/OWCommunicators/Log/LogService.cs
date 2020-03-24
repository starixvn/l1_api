using System.Threading.Tasks;

namespace L1.Features.OWCommunicators.Log
{
    public interface ILogService
    {
        Task LogFailedRequestAsync<T>(OWResponse<T> response) where T : IOWResult;

        Task LogBetAsync<T>(OWResponse<T> response) where T : IOWResult;

        Task LogEndGameAsync<T>(OWResponse<T> response) where T : IOWResult;
    }

    public class LogService : ILogService
    {
        private readonly ILogDataAccess logDataAccess;

        public LogService(ILogDataAccess logDataAccess)
        {
            this.logDataAccess = logDataAccess;
        }

        public async Task LogBetAsync<T>(OWResponse<T> response) where T : IOWResult
        {
            var log = ConvertToOWLog(response);

            await logDataAccess.WriteBetAsync(log);
        }

        public async Task LogEndGameAsync<T>(OWResponse<T> response) where T : IOWResult
        {
            var log = ConvertToOWLog(response);

            await logDataAccess.WriteEndGameAsync(log);
        }

        public async Task LogFailedRequestAsync<T>(OWResponse<T> response) where T : IOWResult
        {
            var log = ConvertToOWLog(response);

            await logDataAccess.WriteFailedRequestAsync(log);
        }

        private OWLog ConvertToOWLog<T>(OWResponse<T> response) where T : IOWResult
            => new OWLog(response.Request.TimeStamp, response.Request.Call, response.TimeStamp, response.Result);
    }
}