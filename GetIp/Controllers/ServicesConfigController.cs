using GetIp.Ultils;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace GetIp.Controllers
{
    [RoutePrefix("v1/Services-config")]
    public class ServicesConfigController : ApiController
    {
        [HttpGet]
        [Route("Getmd5")]
        public async Task<HttpResponseMessage> Getmd5()
        {
            var TrueClientIp = UltilsHelper.GetClientIp();

            var md5 = CookieHelper.InitCookieIp(TrueClientIp);

            return CreateResponse(md5);
        }

        [HttpPost]
        [Route("ReadMd5")]
        public async Task<HttpResponseMessage> ReadMd5(string md5)
        {
            var ip = CookieHelper.GetIpByCookie(md5);

            return CreateResponse(ip);
        }

        private static HttpResponseMessage CreateResponse(Object objectResponse, bool isAddCookie = false, string cookieName = "", string cookieValue = "")
        {
            var response = new HttpResponseMessage();
            var contentStr = string.Empty;
            if (objectResponse != null) contentStr = JsonConvert.SerializeObject(objectResponse);
            response.Content = new StringContent(contentStr);
            return response;
        }
    }
}