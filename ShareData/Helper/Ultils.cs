using ShareData.ErrorCode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace ShareData.Helper
{
    public class Ultils
    {
        public string GetClientIp()
        {
            string text = string.Empty;
            try
            {
                //text = HttpContext.Current.Request.Headers["HTTP_X_FORWARDED_FOR"];
                //bool flag = string.IsNullOrEmpty(text) && HttpContext.Current.Request.Url.Host.IndexOf("localhost") >= 0;
                //if (flag)
                //{
                //    text = "127.0.0.1";
                //}
                //bool flag2 = string.IsNullOrEmpty(text) && HttpContext.Current.Request.Headers["HTTP_CITRIX"] != null;
                //if (flag2)
                //{
                //    text = HttpContext.Current.Request.Headers["HTTP_CITRIX"];
                //}
                //bool flag3 = string.IsNullOrEmpty(text) && HttpContext.Current.Request.Headers["CITRIX_CLIENT_HEADER"] != null;
                //if (flag3)
                //{
                //    text = HttpContext.Current.Request.Headers["CITRIX_CLIENT_HEADER"];
                //}
                //bool flag4 = string.IsNullOrEmpty(text) && HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] != null;
                //if (flag4)
                //{
                //    text = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                //}
            }
            catch
            {
            }
            string[] array = text.Split(new char[]
            {
                ',',
                ':',
                ';'
            });
            bool flag5 = array.Length > 1;
            if (flag5)
            {
                text = array[0];
            }
            return text.Replace('|', '#').Trim();
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

        public static string GetMessageByErrorCode(int errorCode)
        {
            string message = "";
            switch (errorCode)
            {
                case EStatusCode.SUCCESS:
                    message = "Thành công";
                    break;
                case EStatusCode.ACCOUNT_BLOCK:
                    message = "Tài khoản của bạn đã bị khóa!";
                    break;
                case EStatusCode.SYSTEM_ERROR:
                    message = "Đã có lỗi xảy ra!";
                    break;
                case EStatusCode.DATA_INVAILD:
                    message = "Dữ liệu không hợp lệ!";
                    break;
                case EStatusCode.USER_NOT_LOGIN:
                    message = "Bạn cần đăng nhập để sử dụng chức năng này!";
                    break;
                case EStatusCode.SYSTEM_MAINTAIN:
                    message = "Hệ thống đang bảo trì, vui lòng quay lại sau ít phút!";
                    break;
                case EStatusCode.NICKNAME_INVAILD:
                    message = "Nickname không hợp lệ";
                    break;
                case EStatusCode.LOGIN_EXPIRED:
                    message = "Phiên đăng nhập hết hạn";
                    break;
                case EStatusCode.KOL_BLOCK:
                    message = "Idol đã được mời đi uống nước!";
                    break;
                case EStatusCode.CONNECT_ERROR:
                    message = "Kết nối thất bại!";
                    break;
                case EStatusCode.NOT_ENOUGH_RIGHTS:
                    message = "Không đủ quyền";
                    break;
                case EStatusCode.HAVE_ADD_FRIEND:
                    message = "Đã kết bạn!";
                    break;
                case EStatusCode.TOKEN_EXPIRES:
                    message = "Token hết hạn!";
                    break;
                case EStatusCode.SYSTEM_EXCEPTION:
                    message = "Đã có lỗi xảy ra!";
                    break;
                case EStatusCode.NOT_ENOUGH_MONEY:
                    message = "Số dư không đủ!";
                    break;
            }
            return message;
        }
    }
}
