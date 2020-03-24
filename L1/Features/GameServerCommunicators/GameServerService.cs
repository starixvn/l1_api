using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace L1.Features.GameServerCommunicators
{
    public class GameServerService
    {
        private readonly HttpClient httpClient;

        public GameServerService(HttpClient httpClient)
        {
            this.httpClient = httpClient;

        }

    }
}
