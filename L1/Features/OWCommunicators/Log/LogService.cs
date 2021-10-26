using System.Threading.Tasks;
using L1.Shared.Constants;

namespace L1.Features.OWCommunicators.Log
{
    public interface ILogService
    {
        Task LogFailedRequestAsync<T>(OWResponse<T> response) where T : OWResult;

        Task LogBetAsync<T>(OWResponse<T> response, byte system) where T : OWResult;

        Task LogEndGameAsync<T>(OWResponse<T> response) where T : OWResult;

        Task LogVoidGameAsync<T>(OWResponse<T> response) where T : OWResult;

        Task LogSignInAsync(SBLog log);

        Task LogRetrieveEndGameAsync(SBLog log);
    }

    public class LogService : ILogService
    {
        private readonly ILogDataAccess logDataAccess;

        public LogService(ILogDataAccess logDataAccess)
        {
            this.logDataAccess = logDataAccess;
        }

        public async Task LogBetAsync<T>(OWResponse<T> response, byte system) where T : OWResult
        {
            var log = ConvertToOWLog(response, system);

            await logDataAccess.WriteBetAsync(log);
        }

        public async Task LogEndGameAsync<T>(OWResponse<T> response) where T : OWResult
        {
            var log = ConvertToOWLog(response);

            await logDataAccess.WriteEndGameAsync(log);
        }

        public async Task LogVoidGameAsync<T>(OWResponse<T> response) where T : OWResult
        {
            var log = ConvertToOWLog(response);

            await logDataAccess.WriteVoidGameAsync(log);
        }

        public async Task LogFailedRequestAsync<T>(OWResponse<T> response) where T : OWResult
        {
            var log = ConvertToOWLog(response);

            await logDataAccess.WriteFailedRequestAsync(log);
        }

        public Task LogSignInAsync(SBLog log)
            => logDataAccess.WriteSignInAsync(log);

        public Task LogRetrieveEndGameAsync(SBLog log)
            => logDataAccess.WriteRetrieveEndGameAsync(log);

        private static OWLog ConvertToOWLog<T>(OWResponse<T> response, byte system = AppTypes.SabaClub) where T : OWResult
        {
            var call = new OWCall(response.Request.Call.Seq, response.Request.Call.ObCustId, response.Request.Call.SiteId);

            return new OWLog(response.Request.TimeStamp, call, response.TimeStamp, response.Result, system, response.AccessKey);
        }
    }
}