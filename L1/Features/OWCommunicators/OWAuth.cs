using System;

namespace L1.Features.OWCommunicators
{
    public interface IOWAuth
    {
        string Login { get; }

        string Password { get; }
    }

    public class OWAuth : IOWAuth
    {
        public OWAuth(string login, string password)
        {
            Login = login ?? throw new ArgumentNullException(nameof(login));
            Password = password ?? throw new ArgumentNullException(nameof(password));
        }

        public string Login { get; }

        public string Password { get; }
    }
}