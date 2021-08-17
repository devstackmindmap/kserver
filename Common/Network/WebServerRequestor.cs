using AkaSerializer;
using AkaThreading;
using CommonProtocol;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Network
{
    public class WebServerRequestor
    {
        SemaphoreType _semaphoreType;

        public WebServerRequestor(SemaphoreType semaphoreType = SemaphoreType.BattleServer2GameServerBalancer)
        {
            _semaphoreType = semaphoreType;
        }

        public async Task<byte[]> RequestAsync(MessageType messageType, byte[] reqData, string webUrl)
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
                byte[] result = null;

                using (var balancer = await SemaphoreManager.LockAsync(_semaphoreType))
                {
                    result = await webClient.UploadDataTaskAsync(new Uri(webUrl)
                    + messageType.ToString(), "POST"
                    , reqData);
                }

                return result;
            }
        }

        public async Task<TResult>  RequestAsync<TResult>(MessageType messageType, byte[] reqData, string webUrl)
        {
            var rawVal = await RequestAsync(messageType, reqData, webUrl);
            return AkaSerializer<TResult>.Deserialize(rawVal);
        }
    }
}
