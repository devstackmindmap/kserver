using System.Text;

namespace PubSubServer.Channels
{
    public class Manage2AllMessage
    {
        static string GetChannel(string channelHeader)
        {
            return new StringBuilder().Append(channelHeader).ToString();
        }
        
        public static string Sub(string channelHeader)
        {
            return GetChannel(channelHeader);
        }
    }
}
