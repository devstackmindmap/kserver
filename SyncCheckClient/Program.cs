using System;
using CommonProtocol;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Forms;

namespace SyncCheckClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string configPath = string.Format("{0}\\SyncCheckConfig.json", Application.StartupPath);
            
            var jsonData = JObject.Parse(File.ReadAllText(configPath));
            
            var serverList = jsonData["ServerList"];
            string slackToken = jsonData["slackToken"].ToString();
            string slackWebhook = jsonData["slackWebhook"].ToString();
            string urlWithAccessToken = slackWebhook+slackToken;
            var client = new SlackClient(urlWithAccessToken);

            
            foreach (var server in serverList)
            {
               
               string ip = server["IP"].ToString();
               int port = (int)server["Port"];
               string restartURL = server["RestartURL"].ToString();
               string name = server["name"].ToString();

                Connector.Instance.Connect(ip, port);

                var waitDateTime = DateTime.UtcNow;
                var waitDateTimeAfter5second = DateTime.UtcNow.AddSeconds(5);


                bool isConnect = false;
                while (waitDateTime < waitDateTimeAfter5second)
                {
                    waitDateTime = DateTime.UtcNow;

                    if (Connector.Instance.IsConnected())
                    {
                        isConnect = true;
                        break;
                    }
                }
                

                if (false == isConnect)
                {
                    Console.WriteLine($"BattleServerConnectFail:{ip}");
                    return;
                }


                Connector.Instance.Send(MessageType.SyncTime, AkaSerializer.AkaSerializer<ProtoOnSyncTime>.Serialize(new ProtoOnSyncTime
                {
                    MessageType = MessageType.SyncTime,
                    ServerTime = DateTime.UtcNow.Ticks
                }));

                waitDateTime = DateTime.UtcNow;
                waitDateTimeAfter5second = DateTime.UtcNow.AddSeconds(5);
                bool isReceive = false;
                while (waitDateTime < waitDateTimeAfter5second)
                {
                    if (Connector.IsReceive)
                    {
                        Console.WriteLine($"Packet Send And Receive Success:{ip}");
                        isReceive = true;
                        break;
                    }

                    waitDateTime = DateTime.UtcNow;
                }
                //isReceive = false;
                if(!isReceive) { 

                Console.WriteLine($"Packet Send And Receive Fail:{ip}");
                client.PostMessage(username: "김종희",
                           text: $"<@UE6UCFE3F> <@U9N70EYJY> Packet Send And Receive Fail:{ip}",
                           channel: "#system_message");


                    Console.WriteLine(webcall(restartURL));
                    //Console.WriteLine(restartURL);
                  client.PostMessage(username: "김종희",
                       text: $"{name} 서버를 리스타트 했습니다.",
                   channel: "#system_message");
                }
            }


         
        }

        static string webcall(string url)
        {

            string responseText = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = 10 * 1000;
            request.Headers.Add("Authorization", "BASIC SGVsbG8=");
            request.UseDefaultCredentials = true;
            request.PreAuthenticate = true;
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Credentials = new NetworkCredential("akastudio", "dkzktmxbeldh");

            using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
            {
                HttpStatusCode status = resp.StatusCode;
                Console.WriteLine(status);  // 정상이면 "OK"

                Stream respStream = resp.GetResponseStream();
                using (StreamReader sr = new StreamReader(respStream))
                {
                    responseText = sr.ReadToEnd();

                }
            }
            return responseText;
        }
    }
    public class SlackClient
    {
        private readonly Uri _uri;
        private readonly Encoding _encoding = new UTF8Encoding();

        public SlackClient(string urlWithAccessToken)
        {
            _uri = new Uri(urlWithAccessToken);
        }

        //Post a message using simple strings
        public void PostMessage(string text, string username = null, string channel = null)
        {
            Payload payload = new Payload()
            {
                Channel = channel,
                Username = username,
                Text = text
            };

            PostMessage(payload);
        }
        
        //Post a message using a Payload object
        public void PostMessage(Payload payload)
        {
            string payloadJson = JsonConvert.SerializeObject(payload);

            using (WebClient client = new WebClient())
            {
                NameValueCollection data = new NameValueCollection();
                data["payload"] = payloadJson;

                var response = client.UploadValues(_uri, "POST", data);

                //The response text is usually "ok"
                string responseText = _encoding.GetString(response);
            }
        }
    }

    //This class serializes into the Json payload required by Slack Incoming WebHooks
    public class Payload
    {
        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public class Serverlist
    {
        [JsonProperty("IP")]
        public string IP { get; set; }

        [JsonProperty("Port")]
        public int Port { get; set; }

        [JsonProperty("RestartURL")]
        public string RestartURL { get; set; }
    }


}
