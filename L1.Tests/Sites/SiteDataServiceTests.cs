using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using L1.Features.Sites;
using NSubstitute;
using Xunit;

namespace L1.Tests
{
    public class SiteDataServiceTests
    {
        [Theory, AutoData]
        public void GetAll_ReturnAllSites(IEnumerable<Site> data)
        {
            //Arrange
            var siteDataAccess = Substitute.For<ISiteDataAccess>();
            siteDataAccess.GetAll().Returns(data);

            var siteDataService = new SiteDataService(siteDataAccess);

            //Act
            var result = siteDataService.GetAll();

            //Assert
            Assert.Equal(data, result);
        }

        [Theory, AutoData]
        public void GetByClientId_ReturnSite(Site data)
        {
            //Arrange
            var siteDataAccess = Substitute.For<ISiteDataAccess>();
            siteDataAccess.GetByClientId("123").Returns(data);

            var siteDataService = new SiteDataService(siteDataAccess);

            //Act
            var result = siteDataService.GetByClientId("123");

            //Assert
            Assert.Equal(data, result);
        }

        [Theory, AutoData]
        public void GetByClientId_NotExistedClientId_ReturnNull(Site data)
        {
            //Arrange
            var siteDataAccess = Substitute.For<ISiteDataAccess>();
            siteDataAccess.GetByClientId("123").Returns(data);

            var siteDataService = new SiteDataService(siteDataAccess);

            //Act
            var result = siteDataService.GetByClientId("456");

            //Assert
            Assert.Null(result);
        }

        [Theory, AutoData]
        public void GetBySiteId_ReturnSite(Site data)
        {
            //Arrange
            var siteDataAccess = Substitute.For<ISiteDataAccess>();
            siteDataAccess.GetBySiteId("lumi").Returns(data);

            var siteDataService = new SiteDataService(siteDataAccess);

            //Act
            var result = siteDataService.GetBySiteId("lumi");

            //Assert
            Assert.Equal(data, result);
        }

        [Theory, AutoData]
        public void GetBySiteId_NotExistedSiteId_ReturnNull(Site data)
        {
            //Arrange
            var siteDataAccess = Substitute.For<ISiteDataAccess>();
            siteDataAccess.GetBySiteId("lumi").Returns(data);

            var siteDataService = new SiteDataService(siteDataAccess);

            //Act
            var result = siteDataService.GetBySiteId("lumi1");

            //Assert
            Assert.Null(result);
        }

        [Theory, AutoData]
        public void VerifyClient_ValidClient_ReturnTrue(string clientId, string clientSecret)
        {
            //Arrange
            var siteDataAccess = Substitute.For<ISiteDataAccess>();
            siteDataAccess.VerifyClient(clientId,clientSecret).Returns(true);

            var siteDataService = new SiteDataService(siteDataAccess);

            //Act
            var result = siteDataService.VerifyClient(clientId, clientSecret);

            //Assert
            Assert.True(result);
        }


        [Theory, AutoData]
        public async Task VerifyClientWithCurrency_ValidClient_ReturnTrue(string clientId, string clientSecret, string currency)
        {
            //Arrange
            var siteDataAccess = Substitute.For<ISiteDataAccess>();
            siteDataAccess.VerifyClient(clientId, clientSecret, currency).Returns(true);

            var siteDataService = new SiteDataService(siteDataAccess);

            //Act
            var result = await siteDataService.VerifyClient(clientId, clientSecret, currency);

            //Assert
            Assert.True(result);
        }
    }
}