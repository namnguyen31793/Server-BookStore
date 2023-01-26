using Newtonsoft.Json;
using ShareData.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace UtilsSystem.Utils
{
    public static class WebUtils
    {
        public static HttpResponseMessage CreateResponse(Object objectResponse, bool isAddCookie = false, string cookieName = "", string cookieValue = "", bool compress = false)
        {
            var response = new HttpResponseMessage();
            var contentStr = string.Empty;
            if (objectResponse != null) contentStr = JsonConvert.SerializeObject(objectResponse);
            if (compress) contentStr = Security.Base64Encode(contentStr);
            response.Content = new StringContent(contentStr);
            return response;
        }

        public static HttpResponseMessage CreateResponseServer(Object objectResponse)
        {
            var response = new HttpResponseMessage();
            var contentStr = string.Empty;
            if (objectResponse != null) contentStr = JsonConvert.SerializeObject(objectResponse);
            response.Content = new StringContent(contentStr);
            return response;
        }

        //public static string ReadDataFromTextFile(string urlTextFile)
        //{
        //    var pathConfig = HttpContext.Current.Server.MapPath(urlTextFile);
        //    if (!File.Exists(pathConfig)) return string.Empty;
        //    var data = File.ReadAllText(pathConfig);
        //    return data;
        //}
        //public static async Task<string> ReadDataFromTextFileAsync(string urlTextFile)
        //{
        //    var pathConfig = HttpContext.Current.Server.MapPath(urlTextFile);
        //    if (!File.Exists(pathConfig)) return string.Empty;
        //    var data = File.ReadAllText(pathConfig);
        //    return data;
        //}
    }
}
