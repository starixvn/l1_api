using L1.Shared.Configurations;
using L1.Shared.Middlewares;
using L1.Tests._Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
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
            var configuration = Substitute.For<IConfiguration>();
            var appSettings = services.AddSettings(configuration);
            appSettings.MongoDBSettings = new MongoDBSettings
            {
                ConnectionString = "mongodb://127.0.0.1:27017"
            };
            services.AddServices();
            services.AddMongoDBServices(appSettings);
            services.AddSingleton<ISentryClient, MockSentryClient>();

            // Act
            MemberAuthMiddleware.AddMemberAuthentications(services);
            var provider = services.BuildServiceProvider();
            var result = provider.GetRequiredService<IMemberAuthService>();

            // Assert
            Assert.NotNull(result);
        }
    }
}