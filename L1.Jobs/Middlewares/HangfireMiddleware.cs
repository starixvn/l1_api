using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MySql.Core;
using L1.Shared.Configurations;
using L1.Jobs.RecurringTasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace L1.Jobs.Middlewares
{
    public static class HangfireMiddleware
    {
        public static void RegisterHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(
               globalConfiguration
               =>
               {
                   globalConfiguration
                       .UseStorage(new MySqlStorage(configuration.GetConnectionString("Hangfire")));
               });

            services.AddScoped<IBackupTaiXiuTask, BackupTaiXiuTask>();
        }

        public static void UseHangfire(this IApplicationBuilder app, AppSettings appSettings)
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new AllowedAnonymouseAuthorizationFilter() }
            });
            app.UseHangfireServer();

            RegisterTask<IBackupTaiXiuTask>("0 * * * *", job => job.Run());
        }

        private static void RegisterTask<T>(string cronExpression, Expression<Func<T, Task>> methodCall)
        {
            if (!string.IsNullOrWhiteSpace(cronExpression))
            {
                var info = (MethodCallExpression)methodCall.Body;

                RecurringJob.AddOrUpdate(
                            info.Method.Name,
                            methodCall,
                            cronExpression);
            }
        }
    }

    public class AllowedAnonymouseAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}