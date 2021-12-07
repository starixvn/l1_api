using System.Threading.Tasks;
using L1.Shared.Constants;

namespace L1.Features.PartnerCommunicators.Log
{
    public interface ILogService
    {
        Task LogFailedRequestAsync<T>(string partnerId, PartnerResponse<T> response) where T : PartnerResult;

        Task LogBetAsync<T>(string partnerId, PartnerResponse<T> response, byte system) where T : PartnerResult;

        Task LogEndGameAsync<T>(string partnerId, PartnerResponse<T> response) where T : PartnerResult;

        Task LogVoidGameAsync<T>(string partnerId, PartnerResponse<T> response) where T : PartnerResult;

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

        public async Task LogBetAsync<T>(string partnerId, PartnerResponse<T> response, byte system) where T : PartnerResult
        {
            var log = ConvertLog(partnerId, response, system);

            await logDataAccess.WriteBetAsync(log);
        }

        public async Task LogEndGameAsync<T>(string partnerId, PartnerResponse<T> response) where T : PartnerResult
        {
            var log = ConvertLog(partnerId, response);

            await logDataAccess.WriteEndGameAsync(log);
        }

        public async Task LogVoidGameAsync<T>(string partnerId, PartnerResponse<T> response) where T : PartnerResult
        {
            var log = ConvertLog(partnerId, response);

            await logDataAccess.WriteVoidGameAsync(log);
        }

        public async Task LogFailedRequestAsync<T>(string partnerId, PartnerResponse<T> response) where T : PartnerResult
        {
            var log = ConvertLog(partnerId, response);

            await logDataAccess.WriteFailedRequestAsync(log);
        }

        public Task LogSignInAsync(SBLog log)
            => logDataAccess.WriteSignInAsync(log);

        public Task LogRetrieveEndGameAsync(SBLog log)
            => logDataAccess.WriteRetrieveEndGameAsync(log);

        private static PartnerLog ConvertLog<T>(string partnerId, PartnerResponse<T> response, byte system = AppTypes.SabaClub) where T : PartnerResult
        => new PartnerLog(
            partnerId, 
            response.Request.TimeStamp, 
            response.Request.Call, 
            response.TimeStamp, 
            response.Result, 
            system, 
            response.AccessKey
            );
    }
}