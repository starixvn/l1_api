using System.Collections.Generic;
using AutoFixture;
using L1.Features.BackendAuthentications;
using L1.Shared.Configurations;
using Xunit;

namespace L1.Tests.Features.BackendAuthentications
{
    public class AppSettingsTests
    {
        private readonly Fixture fixture = new Fixture();

        public AppSettingsTests()
        {
        }

        [Fact]
        public void BackendAuthConfigs_Get_Always_Return_Value_From_Set()
        {
            // Arrange
            var appSetting = new AppSettings();
            var expected = fixture.Create<BackendAuthConfigs>();
            appSetting.BackendAuthConfigs = expected;

            // Act
            var actual = appSetting.BackendAuthConfigs;

            // Assert
            Assert.Equal(expected, actual);          
        }

        [Fact]
        public void IdentityAudiences_Get_Always_Return_Value_From_Set()
        {
            // Arrange
            var appSetting = new AppSettings();
            var expected = fixture.Create<IEnumerable<string>>();
            appSetting.IdentityAudiences = expected;

            // Act
            var actual = appSetting.IdentityAudiences;

            // Assert
            Assert.Equal(expected, actual);          
        }
    }
}