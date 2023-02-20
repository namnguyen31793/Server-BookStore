using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GetIp.Ultils
{
    public class CookieHelper
    {
        public static string InitCookieIp(string ip)
        {
            var response = Security.Encrypt("@GamaApp2023", ip);
            string result = response.Replace("=", "_");
            return result;
        }

        public static string GetIpByCookie(string accountInfoTxtRaw)
        {
            try
            {
                accountInfoTxtRaw = accountInfoTxtRaw.Split(',')[0];
                accountInfoTxtRaw = accountInfoTxtRaw.Replace("_", "=");
                var ip = Security.Decrypt("@GamaApp2023", accountInfoTxtRaw);

                if (IsIP(ip))
                    return ip;
                else
                    return "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        private static Boolean IsIP(String value)
        {
            if (String.IsNullOrEmpty(value))
                return false;

            var items = value.Split('.');

            if (items.Length != 4)
                return false;

            // Simplest: you may want use, say, NumberStyles.AllowHexSpecifier to allow hex as well
            return items.All(item => byte.TryParse(item, out _));
        }
    }
}