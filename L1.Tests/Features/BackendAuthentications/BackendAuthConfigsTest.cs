using AutoFixture;
using L1.Features.BackendAuthentications;
using Xunit;

namespace L1.Tests.Features.BackendAuthentications
{
    public class BackendAuthConfigsTest
    {
        private readonly Fixture fixture;
        private readonly BackendAuthConfigs configs;

        public BackendAuthConfigsTest()
        {
            fixture = new Fixture();
            configs = new BackendAuthConfigs();
        }

        [Fact]
        public void Address_Get_Always_Return_Value_From_Set()
        {
            // Arrange              
            configs.Address = fixture.Create<string>(); ;
      

            // Act
            var actual = configs.Address;

            // Assert
            Assert.Equal(configs.Address, actual);
        }

        [Fact]
        public void ClientId_Get_Always_Return_Value_From_Set()
        {
            // Arrange     
            configs.ClientId = fixture.Create<string>();


            // Act
            var actual = configs.ClientId;

            // Assert
            Assert.Equal(configs.ClientId, actual);
        }


        [Fact]
        public void ClientSecret_Get_Always_Return_Value_From_Set()
        {
            // Arrange    
           configs.ClientSecret = fixture.Create<string>();


            // Act
            var actual = configs.ClientSecret;

            // Assert
            Assert.Equal(configs.ClientSecret, actual);
        }

        [Fact]
        public void Scope_Get_Always_Return_Value_From_Set()
        {
            // Arrange
            configs.Scope = fixture.Create<string>();


            // Act
            var actual = configs.Scope;

            // Assert
            Assert.Equal(configs.Scope, actual);
        }
    }
}