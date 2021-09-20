using AutoFixture.Xunit2;
using Xunit;

namespace L1.Features.AdminApiCommunicators
{
    public class AdminApiResultTests
    {
        [Theory, AutoData]
        public void Message_Always_ReturnsErrorDescription(string errorDescription)
        {
            // Arrange
            var result = new AdminApiResult
            {
                ErrorDescription = errorDescription
            };

            // Act
            var actualValue = result.Message;

            // Assert
            Assert.Equal(errorDescription, actualValue);
        }
    }
}