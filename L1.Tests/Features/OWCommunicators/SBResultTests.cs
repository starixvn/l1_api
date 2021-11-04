using Xunit;

namespace L1.Features.OWCommunicators
{
    public class SBResultTests
    {
        private readonly SBResult result;

        public SBResultTests()
        {
            result = new SBResult();
        }

        [Fact]
        public void IsSuccessful_ErrorCodeIsZero_ReturnTrue()
        {
            // Arrange 
            result.ErrorCode = 0;

            // Act
            var actualValue = result.IsSuccessful();

            // Assert
            Assert.True(actualValue);
        }

        [Fact]
        public void IsSuccessful_ErrorCodeIsNotZero_ReturnFalse()
        {
            // Arrange 
            result.ErrorCode = 1;

            // Act
            var actualValue = result.IsSuccessful();

            // Assert
            Assert.False(actualValue);
        }

        [Fact]
        public void IsFailed_ErrorCodeIsZero_ReturnFalse()
        {
            // Arrange 
            result.ErrorCode = 0;

            // Act
            var actualValue = result.IsFailed();

            // Assert
            Assert.False(actualValue);
        }

        [Fact]
        public void IsFailed_ErrorCodeIsNotZero_ReturnTrue()
        {
            // Arrange 
            result.ErrorCode = 1;

            // Act
            var actualValue = result.IsFailed();

            // Assert
            Assert.True(actualValue);
        }
    }
}