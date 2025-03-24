using System.Collections.Generic;

namespace L1.Shared.Constants
{
    internal static class ErrorCodes
    {
        internal const string SuccessCode = "0";
        internal const string InternalError = "SB0001";
        internal const int InsufficientFunds = 3002;
        internal const int NewInsufficientFunds = 6999;

        private static readonly Dictionary<int, int> ErrorCodeConversions = new()
        {
            { 6999, 3002 },
            { 7000, 1001 },
            { 7001, 1001 },
        };

        internal static int CovertErrorCode(int code)
        {
            return ErrorCodeConversions.TryGetValue(code, out int convertedCode) 
                ? convertedCode 
                : code;
        }
    }
}