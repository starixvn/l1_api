using System;
using L1.Features.PFFCommunicators;
using Xunit;

namespace L1.Tests.Features.PFFCommunicators
{
    public class PFFRequestTests
    {
        [Fact]
        public void New_CallIsNull_ThrowsException()
        {
            // Act
            try
            {
                _ = new PFFRequest(null);
            }
            catch (Exception ex)
            {
                Assert.IsType<ArgumentNullException>(ex);
            }
        }
    }
}