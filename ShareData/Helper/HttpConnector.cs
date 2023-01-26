using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace ShareData.Helper
{
    public class HttpConnector
    {
        public static string HttpPostRequest(string url, Dictionary<string, string> postParameters, out HttpStatusCode statusCode, bool isAddCookie = false, string authorization = "")
        {
            string result = null;
            try
            {
                string postData = "";

                foreach (string key in postParameters.Keys)
                {
                    postData += WebUtility.UrlEncode(key) + "="
                            + WebUtility.UrlEncode(postParameters[key]) + "&";
                }

                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                myHttpWebRequest.Method = "POST";
                if (isAddCookie)
                {
                    myHttpWebRequest.Headers.Add("Content-Type", "application/json");
                    myHttpWebRequest.Headers.Add("Authorization", authorization);
                }

                byte[] data = Encoding.ASCII.GetBytes(postData);
                myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
                myHttpWebRequest.ContentLength = data.Length;

                Stream requestStream = myHttpWebRequest.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();

                HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

                Stream responseStream = myHttpWebResponse.GetResponseStream();

                StreamReader myStreamReader = new StreamReader(responseStream, Encoding.Default);

                statusCode = ((HttpWebResponse)myHttpWebResponse).StatusCode;
                result = myStreamReader.ReadToEnd();

                myStreamReader.Close();
                responseStream.Close();

                myHttpWebResponse.Close();
            }
            catch (Exception)
            {
                statusCode = HttpStatusCode.ExpectationFailed;
            }
            return result;
        }
        

        public static string HttpGetRequest(string url, Dictionary<string, string> postParameters, out HttpStatusCode statusCode, bool isAddCookie = false, string authorization = "")
        {
            string result = null;
            try
            {
                string postData = "";

                foreach (string key in postParameters.Keys)
                {
                    postData += WebUtility.UrlEncode(key) + "="
                            + WebUtility.UrlEncode(postParameters[key]) + "&";
                }

                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                myHttpWebRequest.Method = "GET";
                if (isAddCookie)
                {
                    myHttpWebRequest.Headers.Add("Content-Type", "application/json");
                    myHttpWebRequest.Headers.Add("Authorization", authorization);
                }

                byte[] data = Encoding.ASCII.GetBytes(postData);
                myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
                myHttpWebRequest.ContentLength = data.Length;

                Stream requestStream = myHttpWebRequest.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();

                HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

                Stream responseStream = myHttpWebResponse.GetResponseStream();

                StreamReader myStreamReader = new StreamReader(responseStream, Encoding.Default);

                statusCode = ((HttpWebResponse)myHttpWebResponse).StatusCode;
                result = myStreamReader.ReadToEnd();

                myStreamReader.Close();
                responseStream.Close();

                myHttpWebResponse.Close();
            }
            catch (Exception)
            {
                statusCode = HttpStatusCode.ExpectationFailed;
            }
            return result;
        }
    }
}
