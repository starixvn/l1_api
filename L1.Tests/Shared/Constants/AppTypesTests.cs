using Xunit;

namespace L1.Shared.Constants
{
    public class AppTypesTests
    {
        public const byte SabaClub = 1;
        public const byte SabaMobi = 2;
        public const byte MiniSabaClub = 3;

        [Fact]
        public void IsValid_SabaClub_ReturnTrue()
        {
            // Act
            var actualValue = AppTypes.IsValid(SabaClub);

            // Assert
            Assert.True(actualValue);
        }

        [Fact]
        public void IsValid_SabaMobi_ReturnTrue()
        {
            // Act
            var actualValue = AppTypes.IsValid(SabaMobi);

            // Assert
            Assert.True(actualValue);
        }

        [Fact]
        public void IsValid_MiniSabaClub_ReturnTrue()
        {
            // Act
            var actualValue = AppTypes.IsValid(MiniSabaClub);

            // Assert
            Assert.True(actualValue);
        }

        [Fact]
        public void IsValid_NotValidAppType_ReturnFalse()
        {
            // Arrange
            byte appType = 0;

            // Act
            var actualValue = AppTypes.IsValid(appType);

            // Assert
            Assert.False(actualValue);
        }
    }
}