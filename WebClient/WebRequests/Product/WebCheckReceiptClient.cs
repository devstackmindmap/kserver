using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace WebClient
{
    class WebCheckReceiptClient
    {
        public static void Run()
        {
            var receiptCheck = new ReceiptCheck
            {
                platform = "google",
                productID = "1000won",
                purchaseToken = "idbpbifoejhcebdlchafogah.AO-J1OykKTdMA3awtnddgXvkvwBLzMnd0bWpiBGayToKZ225oLFJ_tr3NLjgE-tju6Lesc16QGCfzNBRj1G5hY_3L_jlcTNZBSGNe5QrcndrAwdcye5gAqvy8FdFpoHh6XyJZkAHfm2H"
            };

            string jsonString = JsonConvert.SerializeObject(receiptCheck);

            
            Request("http://172.30.1.221:3000/inappcheck", jsonString, out var result);

            var receiptResult = JsonConvert.DeserializeObject<ReceiptCheckResponse>(result);
        }

        public static bool Request(string url, string jsonStr, out string result)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(jsonStr);
                streamWriter.Flush();
                streamWriter.Close();
            }

            result = "";

            try
            {
                using (var response = httpWebRequest.GetResponse() as HttpWebResponse)
                {
                    if (httpWebRequest.HaveResponse && response != null)
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            result = reader.ReadToEnd();
                        }
                    }
                }

                return true;
            }
            catch (WebException e)
            {
                if (e.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)e.Response)
                    {
                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            string error = reader.ReadToEnd();
                            result = error;
                        }
                    }
                }

                return false;
            }
        }
    }

    public class  ReceiptCheck
    {
        public string purchaseToken;
        public string productID;
        public string platform;
    }

    class ReceiptCheckResponse
    {
        public string Result;
        public string transactionId;
        public string purchaseState;
        public string purchaseDate;
        public string expirationDate;
        public string productId;
    }
}
