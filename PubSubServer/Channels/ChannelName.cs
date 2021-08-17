using System.Text;

namespace PubSubServer
{
    public class ChannelName
    {
        public static string Get(string channelHeader, uint userId)
        {
            return new StringBuilder().Append(channelHeader).Append("|").Append(userId).ToString();
        }
    }
}
