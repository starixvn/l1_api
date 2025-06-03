using Xunit;
using L1.Shared.Constants;

namespace L1.Tests.Shared.Constants
{
    public class AppTypesTests
    {
        [Fact]
        public void IsValid_SabaClub_ReturnTrue()
        {
            // Act
            var actualValue = AppTypes.IsValid(AppTypes.SabaClub);

            // Assert
            Assert.True(actualValue);
        }

        [Fact]
        public void IsValid_SabaMobi_ReturnTrue()
        {
            // Act
            var actualValue = AppTypes.IsValid(AppTypes.SabaMobi);

            // Assert
            Assert.True(actualValue);
        }

        [Fact]
        public void IsValid_MiniSabaClub_ReturnTrue()
        {
            // Act
            var actualValue = AppTypes.IsValid(AppTypes.MiniSabaClub);

            // Assert
            Assert.True(actualValue);
        }

        [Fact]
        public void IsValid_MegaKeno_ReturnTrue()
        {
            // Act
            var actualValue = AppTypes.IsValid(AppTypes.MegaKeno);

            // Assert
            Assert.True(actualValue);
        }

        [Fact]
        public void IsValid_SabaApp_ReturnTrue()
        {
            // Act
            var actualValue = AppTypes.IsValid(AppTypes.SabaApp);

            // Assert
            Assert.True(actualValue);
        }

        [Fact]
        public void IsValid_LivePage_ReturnTrue()
        {
            // Act
            var actualValue = AppTypes.IsValid(AppTypes.LivePage);

            // Assert
            Assert.True(actualValue);
        }

        [Fact]
        public void IsValid_MobileDirect_ReturnTrue()
        {
            // Act
            var actualValue = AppTypes.IsValid(AppTypes.MobileDirect);

            // Assert
            Assert.True(actualValue);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(255)]
        public void IsValid_NotValidAppType_ReturnFalse(byte appType)
        {
            // Act
            var actualValue = AppTypes.IsValid(appType);

            // Assert
            Assert.False(actualValue);
        }
    }
}