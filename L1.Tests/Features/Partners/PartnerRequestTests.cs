using System;
using L1.Features.PartnerCommunicators;
using Xunit;

namespace L1.Tests.Features.Partners
{
    public class PartnerRequestTests
    {
        [Fact]
        public void New_CallIsNull_ThrowsException()
        {
            // Act
            try
            {
                _ = new PartnerRequest(null);
            }
            catch (Exception ex)
            {
                Assert.IsType<ArgumentNullException>(ex);
            }
        }
    }
}