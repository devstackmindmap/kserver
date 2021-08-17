using System.IO;
using System.Net;

namespace AkaUtility
{
    public static class JsonRequestor
    {
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
}

