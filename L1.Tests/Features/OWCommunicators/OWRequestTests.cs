using System;
using L1.Features.OWCommunicators;
using Xunit;

namespace L1.Tests.Features.OWCommunicators
{
    public class OWRequestTests
    {
        [Fact]
        public void New_CallIsNull_ThrowsException()
        {
            // Act
            try
            {
                new OWRequest(null, null);
            }
            catch (Exception ex)
            {
                Assert.IsType<ArgumentNullException>(ex);
            }
        }
    }
}