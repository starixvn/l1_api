using System;
using L1.Features.SboCommunicators;
using Xunit;

namespace L1.Tests.Features.SboCommunicators
{
    public class SboRequestTests
    {
        [Fact]
        public void New_CallIsNull_ThrowsException()
        {
            // Act
            try
            {
                _ = new SboRequest(null);
            }
            catch (Exception ex)
            {
                Assert.IsType<ArgumentNullException>(ex);
            }
        }
    }
}