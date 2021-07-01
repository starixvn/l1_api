using System.Collections.Generic;
using AutoFixture.Xunit2;
using L1.Features.Sites;
using Moq;
using Xunit;

namespace L1.Tests
{
    public class SiteDataServiceTests
    {
        [Theory, AutoData]
        public void GetAll_ReturnAllSites(IEnumerable<Site> data)
        {
            //Arrange
            var siteDataAccessMock = new Mock<ISiteDataAccess>();
            siteDataAccessMock.Setup(x => x.GetAll()).Returns(data);

            var siteDataService = new SiteDataService(siteDataAccessMock.Object);

            //Act
            var result = siteDataService.GetAll();

            //Assert
            Assert.Equal(data, result);
        }
    }
}