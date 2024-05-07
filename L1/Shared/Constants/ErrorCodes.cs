using System.Collections.Generic;

namespace L1.Shared.Constants
{
    internal static class ErrorCodes
    {
        public const string SuccessCode = "0";
        public const string InternalError = "SB0001";
        public const int InsufficientFunds = 3002;
        public const int NewInsufficientFunds = 6999;

        public static Dictionary<int, int> ErrorCodeConversions = new()
        {
            { 6999, 3002 },
            { 7000, 1001 },
            { 7001, 1001 },
        };

        public static int CovertErrorCode(int code)
        {
            if (ErrorCodeConversions.ContainsKey(code))
            {
                return ErrorCodeConversions[code];
            }

            return code;
        }
    }
}