using System;
using L1.Features.OWCommunicators;
using Xunit;

namespace L1.Tests.Features.OWCommunicators
{
    public class OWAuthTests
    {
        [Fact]
        public void New_LoginNull_ThrowsException()
        {
            // Act
            try
            {
                new OWAuth(null, "password");
            }
            catch (Exception ex)
            {
                Assert.IsType<ArgumentNullException>(ex);
            }
        }

        [Fact]
        public void New_PasswordNull_ThrowsException()
        {
            // Act
            try
            {
                new OWAuth("login", null);
            }
            catch (Exception ex)
            {
                Assert.IsType<ArgumentNullException>(ex);
            }
        }
    }
}