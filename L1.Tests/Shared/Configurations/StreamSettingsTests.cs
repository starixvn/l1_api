using L1.Shared.Configurations;
using Xunit;

namespace L1.Tests.Shared.Configurations
{
    public class StreamSettingsTests
    {
        [Fact]
        public void DefaultValues_AreSetCorrectly()
        {
            // Arrange & Act
            var settings = new StreamSettings();

            // Assert

            Assert.Equal(500, settings.PublishTimeoutMs);
            Assert.Equal(1, settings.MaxAgeHours);
            Assert.Equal(100_000, settings.MaxMessages);
            Assert.Equal(1, settings.Replicas);
        }
    }
}