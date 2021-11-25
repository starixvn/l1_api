using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Xunit;

namespace L1.Shared.Middlewares
{
    public class ErrorHandlingMiddlewareTests
    {
        [Fact]
        public void ConfigureExceptionHandler_Always_UseExceptionHandler()
        {
            // Arrange
            var app = Substitute.For<IApplicationBuilder>();

            // Act
            ErrorHandlingMiddleware.ConfigureExceptionHandler(app);

            // Assert
            app.Received().Use(Arg.Any<Func<RequestDelegate, RequestDelegate>>());
        }
    }
}