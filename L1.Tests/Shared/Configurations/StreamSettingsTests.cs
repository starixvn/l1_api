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
            // Basic Configuration
            Assert.Equal(500, settings.PublishTimeoutMs);

            // Enhanced defaults for new streams with ack/nak support
            Assert.Equal("File", settings.StorageType);
            Assert.Equal("Limits", settings.RetentionPolicy);
            Assert.Equal("Old", settings.DiscardPolicy);
            Assert.Equal(24, settings.MaxAgeHours); // Enhanced: better persistence
            Assert.Equal(1_000_000L, settings.MaxMessages); // Enhanced: higher capacity
            Assert.Equal(1_073_741_824L, settings.MaxBytes);
            Assert.Equal(1, settings.Replicas);
            Assert.Equal(120_000L, settings.DuplicateWindowMs);
            Assert.False(settings.NoAck);
            Assert.False(settings.AllowRollup);
            Assert.False(settings.DenyDelete);
            Assert.False(settings.DenyPurge);
        }

        [Fact]
        public void BackwardCompatibilityHelpers_WorkCorrectly()
        {
            // Arrange
            var settingsWithNulls = new StreamSettings
            {
                StorageType = null,
                RetentionPolicy = "",
                DiscardPolicy = null
            };

            // Act & Assert
            Assert.Equal("File", settingsWithNulls.GetEffectiveStorageType());
            Assert.Equal("Limits", settingsWithNulls.GetEffectiveRetentionPolicy());
            Assert.Equal("Old", settingsWithNulls.GetEffectiveDiscardPolicy());

            // Test with valid values
            var settingsWithValues = new StreamSettings
            {
                StorageType = "Memory",
                RetentionPolicy = "WorkQueue",
                DiscardPolicy = "New"
            };

            Assert.Equal("Memory", settingsWithValues.GetEffectiveStorageType());
            Assert.Equal("WorkQueue", settingsWithValues.GetEffectiveRetentionPolicy());
            Assert.Equal("New", settingsWithValues.GetEffectiveDiscardPolicy());
        }
    }
}