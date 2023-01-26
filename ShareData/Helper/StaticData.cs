using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ShareData.Helper
{
    public class StaticData
    {
        private static string secretPassKey = null;
        public static string SecretPassKey
        {
            get
            {
                if (string.IsNullOrEmpty(secretPassKey)){
                    secretPassKey = ConfigurationManager.AppSettings["KeyEncodePassword"];
                    return secretPassKey;
                }
                else {
                    return secretPassKey;
                }
            }
        }

        private static string secretCookieKey = null;
        public static string SecretCookieKey
        {
            get
            {
                if (string.IsNullOrEmpty(secretCookieKey))
                {
                    secretCookieKey = ConfigurationManager.AppSettings["KeyEncodeCookie"];
                    return secretCookieKey;
                }
                else
                {
                    return secretCookieKey;
                }
            }
        }

        private static long timeCookieExpire;
        public static long TimeCookieExpire
        {
            get
            {
                if (timeCookieExpire <= 0)
                {
                    var timeExpireTxt = ConfigurationManager.AppSettings["TimeCookieExpire"];
                    long res;
                    long.TryParse(timeExpireTxt, out res);
                    timeCookieExpire = res > 0 ? res : 14400;
                    return res > 0 ? res : 14400;
                }
                else {
                    return timeCookieExpire;
                }
            }
        }

        public static bool IsInMaintain
        {
            get
            {
                var isAlphaTxt = ConfigurationManager.AppSettings["IsMaintain"];
                if (string.IsNullOrEmpty(isAlphaTxt)) return false;
                if (isAlphaTxt.ToLower().Equals("true")) return true;
                return false;
            }
        }

        private static string[] cookieDomainList = null;
        public static string[] CookieDomainList
        {
            get
            {
                if (cookieDomainList == null || cookieDomainList.Length == 0)
                {
                    string str = ConfigurationManager.AppSettings["CookieDomainList"];
                    cookieDomainList = str.Split(',');
                    return str.Split(',');
                }
                else {
                    return cookieDomainList;
                }
            }
        }

        private static string[] cookieNameList = null;
        public static string[] CookieNameList
        {
            get
            {
                if (cookieNameList == null || cookieNameList.Length == 0)
                {
                    string str = ConfigurationManager.AppSettings["CookieNameList"];
                    cookieNameList = str.Split(',');
                    return str.Split(',');
                }
                else
                {
                    return cookieNameList;
                }
            }
        }

        private static string[] baseApiUrlList = null;
        public static string[] BaseApiUrlList
        {
            get
            {
                if (baseApiUrlList == null || baseApiUrlList.Length == 0)
                {
                    string str = ConfigurationManager.AppSettings["BaseApiUrlList"];
                    baseApiUrlList = str.Split(',');
                    return str.Split(',');
                }
                else
                {
                    return baseApiUrlList;
                }
            }
        }
        private static string[] baseApiConfigList = null;
        public static string[] BaseApiConfigList
        {
            get
            {
                if (baseApiConfigList == null || baseApiConfigList.Length == 0)
                {
                    string str = ConfigurationManager.AppSettings["BaseApiConfigList"];
                    baseApiConfigList = str.Split(',');
                    return str.Split(',');
                }
                else
                {
                    return baseApiConfigList;
                }
            }
        }

        public static int GetCookieInfo(string requestHost, ref string cookieName, ref string cookieDomain, ref string baseApi, ref string baseConfigApi)
        {
            //NLogManager.LogError(requestHost);
            int result = -1;
            string[] domainList = CookieDomainList;
            string[] cookieList = CookieNameList;
            //string[] baseApiUrl = BaseApiUrlList;
            //string[] baseApiConfigUrl = BaseApiConfigList;

            for (int i = 0; i < domainList.Length; i++)
            {
                if (requestHost.Contains(domainList[i]))
                {
                    cookieDomain = domainList[i];
                    cookieName = cookieList[i];
                    //baseApi = baseApiUrl[i];
                    //baseConfigApi = baseApiConfigUrl[i];
                    result = i;
                    break;
                }
            }

            return result;
        }
    }
}
