using L1.Features.OWCommunicators;
using L1.Features.PartnerCommunicators;
using L1.Shared.Configurations;
using NSubstitute;
using Sentry;
using System;
using Xunit;

namespace L1.Tests.Features.PartnerCommunicators
{
    public class PartnerCustomerFactoryTests
    {
        private readonly IServiceProvider serviceProvider;

        private readonly IPartnerCustomerFactory partnerCustomerFactory;
        private readonly IAppSettings appSettings;
        private readonly ISentryClient sentryClient;

        public PartnerCustomerFactoryTests()
        {
            serviceProvider = Substitute.For<IServiceProvider>();
            appSettings = Substitute.For<IAppSettings>();
            sentryClient = Substitute.For<ISentryClient>();
            appSettings.PartnerIds = new PartnerIds
            {
                OW = "OW",
                SBO = "SBO"
            };

            partnerCustomerFactory = new PartnerCustomerFactory(serviceProvider, appSettings, sentryClient);
        }

        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        [InlineData("undefined")]
        [Theory]
        public void GetService_IdIsNullOrWhiteSpacesOrUndefined_ReturnNull(string id)
        {
            // Arrange

            // Act
            var result = partnerCustomerFactory.GetCustomerService(id);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetService_OWPartner_ReturnService()
        {
            // Arrange
            serviceProvider.GetService(Arg.Any<Type>()).Returns(new OWCustomerService(null, null, null, null, null, null));

            // Act
            var result = partnerCustomerFactory.GetCustomerService(appSettings.PartnerIds.OW);

            // Assert
            Assert.IsAssignableFrom<IPartnerCustomerServices>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void GetService_SBOPartner_ReturnService()
        {
            // Arrange
            serviceProvider.GetService(Arg.Any<Type>()).Returns(new OWCustomerService(null, null, null, null, null, null));

            // Act
            var result = partnerCustomerFactory.GetCustomerService(appSettings.PartnerIds.SBO);

            // Assert
            Assert.IsAssignableFrom<IPartnerCustomerServices>(result);
            Assert.NotNull(result);
        }
    }
}