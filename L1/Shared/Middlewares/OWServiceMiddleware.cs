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
            services.AddSingleton(owServiceSettings.Endpoints);
            services.AddScoped<IOWCustomerService, OWCustomerService>();

            services.AddHttpClient<IOWService, OWService>(httpClient =>
            {
                httpClient.BaseAddress = owServiceSettings.BaseUrl;
            })
            .AddPolicyHandler(GetRetryPolicy());
        }

        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            const int numberOfRetries = 6;

            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(message => message.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(numberOfRetries, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}