using System;
using L1.Features.HotdogCommunicators;
using Xunit;

namespace L1.Tests.Features.HotdogCommunicators
{
    public class HotdogCallTests
    {
        [Fact]
        public void New_SeqNull_ThrowsException()
        {
            // Act
            try
            {
                _ = new HotdogCall(null, "123", "siteId");
            }
            catch (Exception ex)
            {
                Assert.IsType<ArgumentNullException>(ex);
            }
        }

        [Fact]
        public void New_SeqNotNull_NotThrowsException()
        {
            // Act
            var result = new HotdogCall("seq", "123", "siteId");

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void New_SiteIdNull_ThrowsException()
        {
            // Act
            try
            {
                _ = new HotdogCall("123", null);
            }
            catch (Exception ex)
            {
                Assert.IsType<ArgumentNullException>(ex);
            }
        }
    }
}