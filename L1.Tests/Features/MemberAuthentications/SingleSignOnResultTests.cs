using L1.Features.MemberAuthentications;
using L1.Shared.Models;
using Xunit;

namespace L1.Tests.Features.MemberAuthentications
{
    public class SingleSignOnResultTests
    {
        [Fact]
        public void ServerErrorResult_Always_ReturnsCorrectInfo()
        {
            // Act
            var actualValue = SingleSignOnResult.ServerErrorResult;

            // Assert
            Assert.True(actualValue?.Code == BaseResult.ServerErrorCode
                && actualValue.Message == string.Empty);
        }
    }
}
