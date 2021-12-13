using System;
using L1.Features.OWCommunicators;
using L1.Features.PartnerCommunicators;
using L1.Shared.Constants;
using NSubstitute;
using Xunit;

namespace L1.Tests.Features.PartnerCommunicators
{
    public class PartnerCustomerFactoryTests
    {
        private readonly IServiceProvider serviceProvider;

        private readonly IPartnerCustomerFactory partnerCustomerFactory;

        public PartnerCustomerFactoryTests()
        {
            serviceProvider = Substitute.For<IServiceProvider>();

            partnerCustomerFactory = new PartnerCustomerFactory(serviceProvider);
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

        [InlineData(PartnerIds.OW)]
        [InlineData(PartnerIds.SBO)]
        [Theory]
        public void GetService_DefinedPartner_ReturnService(string id)
        {
            // Arrange
            serviceProvider.GetService(Arg.Any<Type>()).Returns(new OWCustomerService(null, null, null, null, null, null));

            // Act
            var result = partnerCustomerFactory.GetCustomerService(id);

            // Assert
            Assert.IsAssignableFrom<IPartnerCustomerServices>(result);
            Assert.NotNull(result);
        }
    }
}