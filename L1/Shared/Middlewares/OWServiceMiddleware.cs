using System;
using System.Net.Http;
using L1.Features.OWCommunicators;
using L1.Shared.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace L1.Shared.Middlewares
{
    public static class OWServiceMiddleware
    {
        public static void AddOWServices(this IServiceCollection services, OWServiceSettings owServiceSettings)
        {
            services.AddSingleton(new OWAuth(owServiceSettings.Login, owServiceSettings.Password));
            services.AddScoped<IOWCustomerService, OWCustomerService>();

            services.AddHttpClient<IOWService, OWService>(httpClient =>
            {
                httpClient.BaseAddress = owServiceSettings.BaseUrl;
            })
            .AddPolicyHandler(GetRetryPolicy());
        }

        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}