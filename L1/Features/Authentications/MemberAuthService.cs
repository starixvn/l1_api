using System;
using System.Linq;
using L1.Shared.Configurations;
using L1.Shared.Models;

namespace L1.Features.Authentications
{
    public interface IMemberAuthService
    {
        SingleSignOnResult SignIn(MemberAuth memberAuth);
    }

    public class MemberAuthService : IMemberAuthService
    {
        private readonly IMemberAuthDataAccess memberAuthDataAccess;
        private readonly IAppSettings appSettings;

        public MemberAuthService(
            IMemberAuthDataAccess memberAuthDataAccess,
            IAppSettings appSettings)
        {
            this.memberAuthDataAccess = memberAuthDataAccess;
            this.appSettings = appSettings;
        }

        public SingleSignOnResult SignIn(MemberAuth memberAuth)
        {
            var gameServerUrl = GetGameServerUrl(memberAuth);

            if (string.IsNullOrWhiteSpace(gameServerUrl))
            {
                return SingleSignOnResult.CreateErrorResponse(SingleSignOnResult.GameServerNotFoundCode);
            }

            memberAuth.Token = Guid.NewGuid().ToString();
            memberAuth.Time = DateTime.Now;

            memberAuthDataAccess.Create(memberAuth);

            return SingleSignOnResult.CreateSuccessResponse($"{gameServerUrl.TrimEnd('/')}/?token={memberAuth.Token}");
        }

        private string GetGameServerUrl(MemberAuth memberAuth)
        {
            var getLicenseeConfig = appSettings.LicenseeConfigs.FirstOrDefault(config => config.ClientId == memberAuth.SiteId);

            return getLicenseeConfig?.GameServerUrl;
        }

    }
}