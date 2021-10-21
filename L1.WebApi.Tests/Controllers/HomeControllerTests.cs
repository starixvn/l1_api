using L1.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace L1.WebApi.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly HomeController controller;

        public HomeControllerTests()
        {
            controller = new HomeController();
        }

        [Fact]
        public void Index_Always_ReturnCorrectResult()
        {
            // Arrange 
            var expectedResult = new ContentResult
            {
                Content = "Api is running!!!",
                ContentType = "text/html"
            };

            // Act
            var actualValue = controller.Index() as ContentResult;

            // Assert
            Assert.True(expectedResult.Content == actualValue.Content
                && expectedResult.ContentType == actualValue.ContentType);
        }
    }
}