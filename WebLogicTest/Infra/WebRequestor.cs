using CommonProtocol;
using System;
using System.Net;
using System.Threading.Tasks;

namespace WebLogicTest
{
    public class WebServerRequestor
    {
        public WebServerRequestor()
        {

        }

        public byte[] Request(MessageType messageType, byte[] reqData, string webUrl)
        {
            var webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            return webClient.UploadData(new Uri(webUrl)
                + messageType.ToString(), "POST"
                , reqData);
        }

        public async Task<byte[]> RequestAsync(MessageType messageType, byte[] reqData, string webUrl)
        {
            var webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            byte[] result = null;

            result = await webClient.UploadDataTaskAsync(new Uri(webUrl)
            + messageType.ToString(), "POST"
            , reqData);
            return result;
        }
    }
}
