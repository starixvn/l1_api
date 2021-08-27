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
        public void IsUnderMaintenance_StartTimeEqualEndTime_ReturnsFalse()
        {
            // Arrange
            var maintenanceResult = new MaintenanceResult
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now,
            };

            // Act
            var result = maintenanceResult.IsUnderMaintenance();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsUnderMaintenance_StartTimeLessThanEndTime_ReturnsTrue()
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
        public void CalculateDuration_StartTimeIsNull_ReturnsTimeSpanZero()
        {
            // Arrange
            var maintenanceResult = new MaintenanceResult
            {
                StartTime = DateTime.Now
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