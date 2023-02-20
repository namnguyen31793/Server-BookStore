using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GetIp.Ultils
{
    public static class UltilsHelper
    {
        public static string GetClientIp()
        {
            string text = string.Empty;
            try
            {
                text = HttpContext.Current.Request.Headers["HTTP_X_FORWARDED_FOR"];
                bool flag = string.IsNullOrEmpty(text) && HttpContext.Current.Request.Url.Host.IndexOf("localhost") >= 0;
                if (flag)
                {
                    text = "127.0.0.1";
                }
                bool flag2 = string.IsNullOrEmpty(text) && HttpContext.Current.Request.Headers["HTTP_CITRIX"] != null;
                if (flag2)
                {
                    text = HttpContext.Current.Request.Headers["HTTP_CITRIX"];
                }
                bool flag3 = string.IsNullOrEmpty(text) && HttpContext.Current.Request.Headers["CITRIX_CLIENT_HEADER"] != null;
                if (flag3)
                {
                    text = HttpContext.Current.Request.Headers["CITRIX_CLIENT_HEADER"];
                }
                bool flag4 = string.IsNullOrEmpty(text) && HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] != null;
                if (flag4)
                {
                    text = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
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

    }
}