using System.Collections.Generic;
using L1.Features.GameConfigurations;
using L1.Shared.Middlewares;
using L1.Tests._Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sentry;
using Xunit;

namespace L1.Features.MemberAuthentications
{
    public class MemberAuthMiddlewareTests
    {
        [Fact]
        public void AddMemberAuthentications_Always_AddIMemberAuthService()
        {
            // Arrange
            var services = new ServiceCollection();

            // ✅ Tạo cấu hình thật thay vì mock
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "AppSettings:MongoDBSettings:ConnectionString", "mongodb://127.0.0.1:27017" }
                })
                .Build();

            var appSettings = services.AddSettings(configuration);
            services.AddServices();
            services.AddConfigurationApiServices(new ConfigurationApiSettings { Endpoints = new() });
            services.AddMongoDBServices(appSettings);
            services.AddSingleton<ISentryClient, MockSentryClient>();
            services.AddSingleton<IConfiguration>(x => configuration);

            // Act
            MemberAuthMiddleware.AddMemberAuthentications(services);
            var provider = services.BuildServiceProvider();
            var result = provider.GetRequiredService<IMemberAuthService>();

            // Assert
            Assert.NotNull(result);
        }

    }
}