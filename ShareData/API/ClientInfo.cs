using ShareData.DataEnum;
using ShareData.Helper;
using System;
using System.Net.Http;

namespace ShareData.API
{
    public class ClientInfo
    {
        public EOSType OsType { get; set; } = EOSType.SOURCE_ID_OTHERS;
        public string DeviceId { get; set; }
        public int MerchantId { get; set; }
        public string TrueClientIp { get; set; }
        public int IndexConfig { get; set; }
        public string CookieDomain { get; set; }
        public string CookieName { get; set; }
        public string BaseApiUrl { get; set; }

        public ClientInfo() { 
        }
        public ClientInfo(HttpRequestMessage request)
        {
            var header = request.Headers.GetEnumerator(); 
            while (header.MoveNext())
            {
                var c = header.Current;
                if (string.IsNullOrEmpty(c.Key)) continue;
                var v = c.Value.GetEnumerator();
                while (v.MoveNext())
                {
                    if (c.Key.ToLower().Equals("os-type"))
                    {
                        var sourceId = v.Current;
                        var osType = 0;
                        Int32.TryParse(sourceId, out osType);
                        if (Enum.IsDefined(typeof(EOSType), osType))
                        {
                            this.OsType = (EOSType)osType;
                        }
                        else
                        {
                            this.OsType = EOSType.SOURCE_ID_OTHERS;
                        }
                    }
                    if (c.Key.ToLower().Equals("device-client-id"))
                    {
                        this.DeviceId = v.Current;
                    }
                    if (string.IsNullOrEmpty(this.DeviceId))
                    {
                        this.DeviceId = "webgl_deviceId";
                    }
                    if (c.Key.ToLower().Equals("merchantid"))
                    {
                        var merchantIdTxt = v.Current;
                        var merchantId = 0;
                        Int32.TryParse(merchantIdTxt, out merchantId);
                        this.MerchantId = merchantId;
                    }
                    this.TrueClientIp = new Ultils().GetClientIp();
                }
            }
            string cookieName = string.Empty; string cookieDomain = string.Empty; string apiUrl = string.Empty; string apiConfigUrl = string.Empty;
            this.IndexConfig = StaticData.GetCookieInfo(request.RequestUri.Host, ref cookieName, ref cookieDomain, ref apiUrl, ref apiConfigUrl);

            this.CookieName = cookieName;
            this.CookieDomain = cookieDomain;
            this.BaseApiUrl = apiUrl;
        }
    }
}
