using System.Collections.Generic;
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
    }
}