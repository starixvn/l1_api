using L1.Shared.Models;

namespace L1.Features.MemberAuthentications
{
    public class VerificationResult : BaseResult
    {
        public const int TokenNotFoundCode = 100;
        public const int TokenExpiredCode = 101;
        public const int InvalidUserAgentOrIPCode = 103;
        public const int NotSupportedCurrency = 104;

        public VerificationResult(int code, string message)
        {
            Code = code;
            Message = message;
        }

#pragma warning disable S107 // Methods should not have too many parameters

        public VerificationResult(
            int code,
            string message,
            string playerId,
            string playerName,
            string siteId,
            string language,
            string currencyName,
            string seq,
            string operatorId,
            string domain,
            string siteName,
            byte appType,
            string currencyCode,
            double multiplier,
            string currencyGroupId,
            bool isTestCurrency,
            bool disableJackpotGames,
            string operatorToken,
            bool isCash
            )
#pragma warning restore S107 // Methods should not have too many parameters
            : this(code, message)
        {
            PlayerId = playerId;
            PlayerName = playerName;
            SiteId = siteId;
            Language = language;
            CurrencyName = currencyName;
            Seq = seq;
            OperatorId = operatorId;
            Domain = domain;
            SiteName = siteName;
            AppType = appType;
            CurrencyCode = currencyCode;
            Multiplier = multiplier;
            CurrencyGroupId = currencyGroupId;
            IsTestCurrency = isTestCurrency;
            OperatorToken = operatorToken;
            DisableJackpotGames = disableJackpotGames;
            IsCash = isCash;
        }

        public string PlayerId { get; }

        public string PlayerName { get; }

        public string SiteId { get; }

        public string OperatorId { get; }

        public string Domain { get; }

        public string Language { get; }

        public string CurrencyName { get; }

        public string CurrencyCode { get; }

        public string CurrencyGroupId { get; }

        public double Multiplier { get; }

        public string Seq { get; }

        public string MemberKey => $"{OperatorId}_{PlayerId}";

        public string SiteName { get; set; }

        public byte AppType { get; set; }

        public bool IsTestCurrency { get; }

        public string OperatorToken { get; }

        public bool DisableJackpotGames { get; }

        public bool IsCash { get; set; }

        public static VerificationResult CreateErrorResponse(int code)
           => new(code, string.Empty);

#pragma warning disable S107 // Methods should not have too many parameters

        public static VerificationResult CreateSuccessResponse(
            string playerId,
            string playerName,
            string siteId,
            string language,
            string currencyName,
            string seq,
            string operatorId,
            string domain,
            string siteName,
            byte appType,
            string currencyCode,
            double multiplier,
            string currencyGroupId,
            bool isTestCurrency,
            bool disableJackpotGames,
            string operatorToken,
            bool isCash)
#pragma warning restore S107 // Methods should not have too many parameters
            => new VerificationResult(
                SuccessCode,
                string.Empty,
                playerId,
                playerName,
                siteId,
                language,
                currencyName,
                seq,
                operatorId,
                domain,
                siteName,
                appType,
                currencyCode,
                multiplier,
                currencyGroupId,
                isTestCurrency,
                disableJackpotGames,
                operatorToken,
                isCash);
    }
}