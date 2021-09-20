using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace L1.Shared.Middlewares
{
    public class LocalizationMiddlewareTests
    {
        private readonly LocalizationMiddleware middleware;
        private readonly HttpContext context;
        private bool isCompletedNext;

        public LocalizationMiddlewareTests()
        {
            context = new DefaultHttpContext();
            RequestDelegate next = (HttpContext hc) =>
            {
                isCompletedNext = true;
                return Task.CompletedTask;
            };

            middleware = new LocalizationMiddleware(next);
        }

        [Fact]
        public async Task Invoke_HeaderNotContainLanguageKey_InvokeNext()
        {
            // Arrange
            isCompletedNext = false;

            // Act
            await middleware.Invoke(context);

            // Assert
            Assert.True(isCompletedNext);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        public async Task Invoke_HeaderContainLanguageKey_InvokeNext(string languageCode)
        {
            // Arrange
            isCompletedNext = false;
            context.Request.Headers["languageCode"] = languageCode;

            // Act
            await middleware.Invoke(context);

            // Assert
            Assert.True(isCompletedNext);
        }
    }
}