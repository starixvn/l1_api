using System;
using L1.Features.OWCommunicators;
using Xunit;

namespace L1.Tests.Features.OWCommunicators
{
    public class OWCallTests
    {
        [Fact]
        public void New_SeqNull_ThrowsException()
        {
            // Act
            try
            {
                new OWCall(null, 123, "siteId");
            }
            catch (Exception ex)
            {
                Assert.IsType<ArgumentNullException>(ex);
            }
        }

        [Fact]
        public void New_SiteIdNull_ThrowsException()
        {
            // Act
            try
            {
                new OWCall("seq", 123, null);
            }
            catch (Exception ex)
            {
                Assert.IsType<ArgumentNullException>(ex);
            }
        }
    }
}