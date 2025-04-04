using L1.Shared.Configurations;
using NATS.Client.JetStream;
using Xunit;

namespace L1.Tests.Shared.Configurations
{
    public class StreamSettingsTests
    {
        [Fact]
        public void GetRetentionPolicy_WhenPolicyIsInterest_ReturnsInterestPolicy()
        {
            // Arrange
            var settings = new StreamSettings
            {
                RetentionPolicy = StreamSettings.RetentionPolicyInterest
            };

            // Act
            var result = settings.GetRetentionPolicy();

            // Assert
            Assert.Equal(RetentionPolicy.Interest, result);
        }

        [Fact]
        public void GetRetentionPolicy_WhenPolicyIsLimits_ReturnsLimitsPolicy()
        {
            // Arrange
            var settings = new StreamSettings
            {
                RetentionPolicy = StreamSettings.RetentionPolicyLimits
            };

            // Act
            var result = settings.GetRetentionPolicy();

            // Assert
            Assert.Equal(RetentionPolicy.Limits, result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("unknown")]
        [InlineData("workqueue")]
        public void GetRetentionPolicy_WhenPolicyIsNotRecognized_ReturnsWorkQueuePolicy(string policy)
        {
            // Arrange
            var settings = new StreamSettings
            {
                RetentionPolicy = policy
            };

            // Act
            var result = settings.GetRetentionPolicy();

            // Assert
            Assert.Equal(RetentionPolicy.WorkQueue, result);
        }

        [Fact]
        public void GetDiscardPolicy_WhenPolicyIsNew_ReturnsNewPolicy()
        {
            // Arrange
            var settings = new StreamSettings
            {
                DiscardPolicy = StreamSettings.DiscardPolicyNew
            };

            // Act
            var result = settings.GetDiscardPolicy();

            // Assert
            Assert.Equal(DiscardPolicy.New, result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("unknown")]
        [InlineData("old")]
        public void GetDiscardPolicy_WhenPolicyIsNotRecognized_ReturnsOldPolicy(string policy)
        {
            // Arrange
            var settings = new StreamSettings
            {
                DiscardPolicy = policy
            };

            // Act
            var result = settings.GetDiscardPolicy();

            // Assert
            Assert.Equal(DiscardPolicy.Old, result);
        }

        [Fact]
        public void DefaultValues_AreSetCorrectly()
        {
            // Arrange & Act
            var settings = new StreamSettings();

            // Assert
            Assert.Equal(StreamSettings.StorageTypeMemory, settings.StorageType);
            Assert.Equal(StreamSettings.RetentionPolicyWorkQueue, settings.RetentionPolicy);
            Assert.Equal(StreamSettings.DiscardPolicyOld, settings.DiscardPolicy);
            Assert.Equal(500, settings.PublishTimeoutMs);
            Assert.Equal(1, settings.MaxAgeHours);
            Assert.Equal(100, settings.MaxSizeMB);
            Assert.Equal(2, settings.DuplicateWindowMinutes);
            Assert.Equal(100_000, settings.MaxMessages);
            Assert.Equal(1, settings.Replicas);
        }
    }
} 