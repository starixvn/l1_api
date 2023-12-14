using System;
using L1.Features.PFFCommunicators;
using Xunit;

namespace L1.Tests.Features.PFFCommunicators
{
    public class PFFCallTests
    {
        [Fact]
        public void New_SeqNull_ThrowsException()
        {
            // Act
            try
            {
                _ = new PFFCall(null, "123", "siteId");
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
            var result = new PFFCall("seq", "123", "siteId");

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void New_SiteIdNull_ThrowsException()
        {
            // Act
            try
            {
                _ = new PFFCall("123", null);
            }
            catch (Exception ex)
            {
                Assert.IsType<ArgumentNullException>(ex);
            }
        }
    }
}