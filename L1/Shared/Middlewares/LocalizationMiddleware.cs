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
            var request = context.Request;
            var headerContainLanguageKey = request.Headers.Keys.Contains(LanguageKey);

            if (headerContainLanguageKey)
            {
                var languageCode = request.Headers[LanguageKey];

                if (!string.IsNullOrWhiteSpace(languageCode))
                {
                    var cultureInfo = new CultureInfo(languageCode);

                    Thread.CurrentThread.CurrentCulture = cultureInfo ?? DefaultCulture;
                    Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

                    Thread.CurrentThread.CurrentCulture.DateTimeFormat = DefaultCulture.DateTimeFormat;
                    Thread.CurrentThread.CurrentCulture.NumberFormat = DefaultCulture.NumberFormat;
                }
            }

            await next(context);
        }
    }

    public static class LocalizationMiddlewareExtensions
    { 
        public static void ConfigureLocalization(this IApplicationBuilder app)
            => app.UseMiddleware(typeof(LocalizationMiddleware));
    }
}