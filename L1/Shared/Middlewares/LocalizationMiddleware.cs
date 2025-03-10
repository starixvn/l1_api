using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace L1.Shared.Middlewares
{
    public class LocalizationMiddleware
    {
        private const string DefaultLanguage = "vi-VN";
        private const string LanguageKey = "languageCode";

        private static readonly CultureInfo DefaultCulture = CultureInfo.CreateSpecificCulture(DefaultLanguage);

        private readonly RequestDelegate next;

        public LocalizationMiddleware(RequestDelegate next)
            => this.next = next;

        public async Task Invoke(HttpContext context)
        {
            var cultureInfo = DefaultCulture;
            var request = context.Request;
            var headerContainsLanguageKey = request.Headers.ContainsKey(LanguageKey);

            if (headerContainsLanguageKey)
            {
                var languageCode = request.Headers[LanguageKey];

                if (!string.IsNullOrWhiteSpace(languageCode))
                {
                    cultureInfo = new CultureInfo(languageCode);
                }
            }

            Thread.CurrentThread.CurrentCulture = cultureInfo ?? DefaultCulture;
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            Thread.CurrentThread.CurrentCulture.DateTimeFormat = DefaultCulture.DateTimeFormat;
            Thread.CurrentThread.CurrentCulture.NumberFormat = DefaultCulture.NumberFormat;

            await next(context);
        }
    }

    public static class LocalizationMiddlewareExtensions
    {
        public static void ConfigureLocalization(this IApplicationBuilder app)
            => app.UseMiddleware(typeof(LocalizationMiddleware));
    }
}