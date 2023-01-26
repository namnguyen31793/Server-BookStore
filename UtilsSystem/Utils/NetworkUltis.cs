using LoggerService;
using ShareData.LogSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UtilsSystem.Utils
{
    public static class NetworkUltis
    {
        public static string SendPost(string postData, string url, out int responseCode)
        {
            responseCode = -99;
            string result = string.Empty;
            try
            {
                UTF8Encoding uTf8Encoding = new UTF8Encoding();
                byte[] bytes = uTf8Encoding.GetBytes(postData);
                ServicePointManager.Expect100Continue = true;
                CookieContainer cookieContainer = new CookieContainer();
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json; charset=UTF-8";
                httpWebRequest.ContentLength = bytes.Length;
                httpWebRequest.KeepAlive = false;
                httpWebRequest.Proxy = new WebProxy();
                httpWebRequest.CookieContainer = cookieContainer;
                httpWebRequest.AllowAutoRedirect = false;
                using (Stream requestStream = httpWebRequest.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }
                using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (Stream responseStream = httpWebResponse.GetResponseStream())
                    {
                        if (responseStream != null)
                            using (StreamReader streamReader = new StreamReader(responseStream))
                            {
                                result = streamReader.ReadToEnd();
                                responseCode = 0;
                            }
                        httpWebResponse.Close();
                    }
                }
            }
            catch (WebException webException)
            {
                if (webException.Status == WebExceptionStatus.ProtocolError)
                {
                    WebResponse resp = webException.Response;
                    using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                    {
                        result = sr.ReadToEnd();
                        responseCode = -1;
                    }
                }
            }
            catch (Exception exception)
            {
                result = exception.ToString();
                responseCode = -2;
            }
            return result;

        }

        public static string SendPostText(string postData, string url, out int responseCode)
        {
            responseCode = -99;
            string result = string.Empty;
            try
            {
                UTF8Encoding uTf8Encoding = new UTF8Encoding();
                byte[] bytes = uTf8Encoding.GetBytes(postData);
                ServicePointManager.Expect100Continue = true;
                CookieContainer cookieContainer = new CookieContainer();
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/x-www-form-urlencoded"; ;// "text/plain; charset=UTF-8";
                httpWebRequest.ContentLength = bytes.Length;
                httpWebRequest.KeepAlive = false;
                httpWebRequest.Proxy = new WebProxy();
                httpWebRequest.CookieContainer = cookieContainer;
                httpWebRequest.AllowAutoRedirect = false;
                using (Stream requestStream = httpWebRequest.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }
                using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (Stream responseStream = httpWebResponse.GetResponseStream())
                    {
                        if (responseStream != null)
                            using (StreamReader streamReader = new StreamReader(responseStream))
                            {
                                result = streamReader.ReadToEnd();
                                responseCode = 0;
                            }
                        httpWebResponse.Close();
                    }
                }
            }
            catch (WebException webException)
            {
                if (webException.Status == WebExceptionStatus.ProtocolError)
                {
                    WebResponse resp = webException.Response;
                    using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                    {
                        result = sr.ReadToEnd();
                        responseCode = -1;
                    }
                }
            }
            catch (Exception exception)
            {
                result = exception.ToString();
                responseCode = -2;
            }
            return result;

        }
        public static string SendPostBase64(string postData, string url, out int responseCode)
        {
            responseCode = -99;
            string result = string.Empty;
            try
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] bytes = encoding.GetBytes(postData);
                string returnValue = System.Convert.ToBase64String(bytes);
                bytes = Encoding.UTF8.GetBytes(returnValue);
                ServicePointManager.Expect100Continue = true;
                CookieContainer cookieContainer = new CookieContainer();
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";// "text/plain; charset=UTF-8";
                httpWebRequest.ContentLength = bytes.Length;
                httpWebRequest.KeepAlive = false;
                httpWebRequest.Proxy = new WebProxy();
                httpWebRequest.CookieContainer = cookieContainer;
                httpWebRequest.AllowAutoRedirect = false;
                using (Stream requestStream = httpWebRequest.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }
                using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (Stream responseStream = httpWebResponse.GetResponseStream())
                    {
                        if (responseStream != null)
                            using (StreamReader streamReader = new StreamReader(responseStream))
                            {
                                result = streamReader.ReadToEnd();
                                responseCode = 0;
                            }
                        httpWebResponse.Close();
                    }
                }
            }
            catch (WebException webException)
            {
                if (webException.Status == WebExceptionStatus.ProtocolError)
                {
                    WebResponse resp = webException.Response;
                    using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                    {
                        result = sr.ReadToEnd();
                        responseCode = -1;
                    }
                }
            }
            catch (Exception exception)
            {
                result = exception.ToString();
                responseCode = -2;
            }
            return result;

        }

        public static string GetStringHttpResponse(string url)
        {
            string result = null;
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "GET";
                httpWebRequest.CookieContainer = new CookieContainer();
                httpWebRequest.ContentType = "text/xml; encoding='utf-8'";
                httpWebRequest.KeepAlive = false;
                using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                    {
                        bool flag = streamReader != null;
                        if (flag)
                        {
                            result = streamReader.ReadToEnd();
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
            }
            return result;
        }

        public static string GetStringHttpResponse(string url, out int responseCode)
        {
            responseCode = -9999;
            string result = null;
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "GET";
                httpWebRequest.CookieContainer = new CookieContainer();
                httpWebRequest.ContentType = "application/json; encoding='utf-8'";
                httpWebRequest.Proxy = new WebProxy();
                httpWebRequest.KeepAlive = false;
                using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                    {
                        bool flag = streamReader != null;
                        if (flag)
                        {
                            result = streamReader.ReadToEnd();
                            responseCode = 0;
                        }
                    }
                }
            }
            catch (WebException webException)
            {
                if (webException.Status == WebExceptionStatus.ProtocolError)
                {
                    WebResponse resp = webException.Response;
                    using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                    {
                        result = sr.ReadToEnd();
                        responseCode = -1;
                    }
                }
            }
            catch (Exception exception)
            {
                result = exception.ToString();
                responseCode = -2;
            }
            return result;
        }

    }
}
