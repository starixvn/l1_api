using System;
using L1.Features.HotdogCommunicators;
using Xunit;

namespace L1.Tests.Features.HotdogCommunicators
{
    public class HotdogRequestTests
    {
        [Fact]
        public void New_CallIsNull_ThrowsException()
        {
            // Act
            try
            {
                _ = new HotdogRequest(null);
            }
            catch (Exception ex)
            {
                Assert.IsType<ArgumentNullException>(ex);
            }
        }
    }
}