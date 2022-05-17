using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using L1.WebApi.Controllers;
using L1.WebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace L1.WebApi.Tests.Controllers
{
    public class PublicControllerTests
    {
        private  readonly Fixture fixture = new();

        [Fact]
        public void HashCheck_ValidModel_ReturnResult()
        {
            //Arrange
            var model = fixture.Create<HashCheck>();
            var controller = new PublicController();

            //Act
            var result = controller.HashCheck(model);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.False(string.IsNullOrWhiteSpace(okResult.Value.ToString()));
        }

        [Fact]
        public void HashCheck_InvalidModel_ReturnBadRequest()
        {
            //Arrange
            var model = fixture.Create<HashCheck>();
            var controller = new PublicController();
            controller.ModelState.AddModelError("Data", "Data is required");
            //Act
            var result = controller.HashCheck(model);

            //Assert
            Assert.IsType<BadRequestResult>(result);
        }
    }
}
