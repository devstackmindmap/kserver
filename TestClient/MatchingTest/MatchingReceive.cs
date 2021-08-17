using CommonProtocol;
using System;

namespace TestClient.MatchingTest
{
    public class MatchingReceive
    {
        public static void Run(BaseProtocol proto, MessageType msgType)
        {
            Console.WriteLine(msgType.ToString());
            
        }
    }
}
