using System;
using L1.Features.OWCommunicators.CheckMaintenance;
using Xunit;

namespace L1.Tests.Features.OWCommunicators.CheckMaintenance
{
    public class MaintenanceResultTests
    {
        [Fact]
        public void IsUnderMaintenance_StartTimeIsNull_ReturnsFalse()
        {
            // Arrange
            var maintenanceResult = new MaintenanceResult();

            // Act
            var result = maintenanceResult.IsUnderMaintenance();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsUnderMaintenance_EndTimeIsNull_ReturnsFalse()
        {
            // Arrange
            var maintenanceResult = new MaintenanceResult
            {
                StartTime = DateTime.Now,
                EndTime = null,
            };

            // Act
            var result = maintenanceResult.IsUnderMaintenance();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsUnderMaintenance_NowInRangeStartTimeAndEndTime_ReturnsTrue()
        {
            // Arrange
            var maintenanceResult = new MaintenanceResult
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddSeconds(30),
            };

            // Act
            var result = maintenanceResult.IsUnderMaintenance();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsUnderMaintenance_NowLessThanStartTime_ReturnsFalse()
        {
            // Arrange
            var maintenanceResult = new MaintenanceResult
            {
                StartTime = DateTime.Now.AddSeconds(30),
                EndTime = DateTime.Now.AddSeconds(30),
            };

            // Act
            var result = maintenanceResult.IsUnderMaintenance();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsUnderMaintenance_NowGreaterThanEndTime_ReturnsFalse()
        {
            // Arrange
            var maintenanceResult = new MaintenanceResult
            {
                StartTime = DateTime.Now.AddSeconds(-30),
                EndTime = DateTime.Now.AddSeconds(-30),
            };

            // Act
            var result = maintenanceResult.IsUnderMaintenance();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CalculateDuration_StartTimeIsNull_ReturnsTimeSpanZero()
        {
            // Arrange
            var maintenanceResult = new MaintenanceResult
            {
                StartTime = null,
                EndTime = DateTime.Now
            };

            // Act
            var result = maintenanceResult.CalculateDuration();

            // Assert
            Assert.Equal(TimeSpan.Zero, result);
        }

        [Fact]
        public void CalculateDuration_EndTimeIsNull_ReturnsTimeSpanZero()
        {
            // Arrange
            var maintenanceResult = new MaintenanceResult
            {
                StartTime = DateTime.Now,
                EndTime = null
            };

            // Act
            var result = maintenanceResult.CalculateDuration();

            // Assert
            Assert.Equal(TimeSpan.Zero, result);
        }

        [Fact]
        public void CalculateDuration_StartTimeAndEndTimeValid_ReturnsDuration()
        {
            // Arrange
            var maintenanceResult = new MaintenanceResult
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddSeconds(30)
            };

            // Act
            var result = maintenanceResult.CalculateDuration();

            // Assert
            Assert.True(result > TimeSpan.FromSeconds(29));
        }
    }
}