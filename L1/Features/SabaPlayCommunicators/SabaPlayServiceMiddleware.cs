using System.Net;
using System;
using System.Net.Http;
using Coravel;
using Microsoft.Extensions.DependencyInjection;
using Coravel.Scheduling.Schedule.Interfaces;
using L1.Features.PartnerCommunicators;
using L1.Features.SabaPlayCommunicators.CheckMaintenance;
using L1.Features.SabaPlayCommunicators.DailySummary;
using L1.Features.SabaPlayCommunicators.ScheduleJob;

namespace L1.Features.SabaPlayCommunicators
{
    public static class SabaPlayServiceMiddleware
    {
        public static void AddSabaPlayServices(this IServiceCollection services, SabaPlayServiceSettings sabaPlayServiceSettings)
        {
            services.AddSingleton(sabaPlayServiceSettings);
            services.AddSingleton(sabaPlayServiceSettings.Endpoints);
            services.AddScoped<ISabaPlayFailureHandler, SabaPlayFailureHandler>();

            services.AddScoped<SabaPlayCustomerService>()
                   .AddScoped<IPartnerCustomerServices, SabaPlayCustomerService>(s => s.GetService<SabaPlayCustomerService>());

            services.AddHttpClient<ISabaPlayMaintenanceService, SabaPlayMaintenanceService>();
            var sabaPlayHttpClient = services.AddHttpClient<ISabaPlayService, SabaPlayService>(httpClient =>
            {
                httpClient.BaseAddress = sabaPlayServiceSettings.BaseUrl;
                httpClient.Timeout = TimeSpan.FromSeconds(30); // Add reasonable timeout
            });

            if (!string.IsNullOrEmpty(sabaPlayServiceSettings.Proxy))
            {
                sabaPlayHttpClient.ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new HttpClientHandler
                    {
                        Proxy = new WebProxy(sabaPlayServiceSettings.Proxy),
                        UseProxy = true,
                    };
                });
            }
        }

        public static void AddSabaPlayDailySummaryServices(this IServiceCollection services,
            SabaPlayDailySummaryServiceSettings sabaPlayDailySummaryServiceSettings)
        {
            services.AddScheduler();
            services.AddTransient<DailySummaryJob>();
            services.AddScoped<IDailySummaryDataAccess, DailySummaryDataAccess>();
            services.AddScoped<ISabaPlayDailySummaryService, SabaPlayDailySummaryService>();

            services.AddSingleton(sabaPlayDailySummaryServiceSettings);

            var sabaPlayHttpClient = services.AddHttpClient<ISabaPlayDailySummaryService, SabaPlayDailySummaryService>(httpClient =>
            {
                httpClient.BaseAddress = sabaPlayDailySummaryServiceSettings.BaseUrl;
            });

            if (!string.IsNullOrEmpty(sabaPlayDailySummaryServiceSettings.Proxy))
            {
                sabaPlayHttpClient.ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new HttpClientHandler
                    {
                        Proxy = new WebProxy(sabaPlayDailySummaryServiceSettings.Proxy),
                        UseProxy = true,
                    };
                });
            }
        }

        public static void AddSabaplayScheduleJobs(this IScheduler scheduler)
        {
            scheduler.Schedule<DailySummaryJob>().Cron("15 0,4,11,18 * * *").RunOnceAtStart().Zoned(System.TimeZoneInfo.Utc);
        }
    }
}