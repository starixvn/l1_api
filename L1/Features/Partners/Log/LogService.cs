using System.Threading.Tasks;
using L1.Shared.Constants;

namespace L1.Features.Partners.Log
{
    public interface ILogService
    {
        Task LogFailedRequestAsync<T>(PartnerResponse<T> response) where T : PartnerResult;

        Task LogBetAsync<T>(PartnerResponse<T> response, byte system) where T : PartnerResult;

        Task LogEndGameAsync<T>(PartnerResponse<T> response) where T : PartnerResult;

        Task LogVoidGameAsync<T>(PartnerResponse<T> response) where T : PartnerResult;

        //Task LogSignInAsync(SBLog log);

        //Task LogRetrieveEndGameAsync(SBLog log);
    }

    public class LogService : ILogService
    {
        private readonly ILogDataAccess logDataAccess;

        public LogService(ILogDataAccess logDataAccess)
        {
            this.logDataAccess = logDataAccess;
        }

        public async Task LogBetAsync<T>(PartnerResponse<T> response, byte system) where T : PartnerResult
        {
            var log = ConvertToOWLog(response, system);

            await logDataAccess.WriteBetAsync(log);
        }

        public async Task LogEndGameAsync<T>(PartnerResponse<T> response) where T : PartnerResult
        {
            var log = ConvertToOWLog(response);

            await logDataAccess.WriteEndGameAsync(log);
        }

        public async Task LogVoidGameAsync<T>(PartnerResponse<T> response) where T : PartnerResult
        {
            var log = ConvertToOWLog(response);

            await logDataAccess.WriteVoidGameAsync(log);
        }

        public async Task LogFailedRequestAsync<T>(PartnerResponse<T> response) where T : PartnerResult
        {
            var log = ConvertToOWLog(response);

            await logDataAccess.WriteFailedRequestAsync(log);
        }

        //public Task LogSignInAsync(SBLog log)
        //    => logDataAccess.WriteSignInAsync(log);

        //public Task LogRetrieveEndGameAsync(SBLog log)
        //    => logDataAccess.WriteRetrieveEndGameAsync(log);

        private static PartnerLog ConvertToOWLog<T>(PartnerResponse<T> response, byte system = AppTypes.SabaClub) where T : PartnerResult
        => new PartnerLog(response.Request.TimeStamp, response.Request.Call, response.TimeStamp, response.Result, system, response.AccessKey);
    }
}