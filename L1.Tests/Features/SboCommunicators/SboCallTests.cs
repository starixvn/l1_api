using System;
using L1.Features.SboCommunicators;
using Xunit;

namespace L1.Tests.Features.SboCommunicators
{
    public class SboCallTests
    {
        [Fact]
        public void New_SeqNull_ThrowsException()
        {
            // Act
            try
            {
                _ = new SboCall(null, "123", "siteId");
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
            var result = new SboCall("seq", "123", "siteId");

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void New_SiteIdNull_ThrowsException()
        {
            // Act
            try
            {
                _ = new SboCall("123", null);
            }
            catch (Exception ex)
            {
                Assert.IsType<ArgumentNullException>(ex);
            }
        }
    }
}