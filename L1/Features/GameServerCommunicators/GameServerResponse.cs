using System;

namespace L1.Features.GameServerCommunicators
{
    public class GameServerResponse<T> where T : GameServerResult
    {
        public DateTimeOffset TimeStamp { get; set; }

        public T Result { get; set; }
    }
}