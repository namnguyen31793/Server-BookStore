using LoggerService;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UtilsSystem.Utils
{
    public class NhanhInstance
    {
        private static NhanhInstance _inst;
        private static object _syncObject = new object();

        public static NhanhInstance Inst
        {
            get
            {
                if (_inst == null)
                    lock (_syncObject)
                    {
                        if (_inst == null) _inst = new NhanhInstance();
                    }
                return _inst;
            }
        }
        public async Task<string> SendServiceIdNhanh(string url, string data)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(30000); //30s
           
            var options = new RestClientOptions(NO_SQL_CONFIG.NHANH_Url)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest( url, Method.Post);
            request.AddHeader("Cookie", "Npos-Csrf-Token-V1=ExWWciuCSv6BtjsJ0Wy8CPkPIkVor8rwhDA5CBQQAaorMrUqDoRfTK4d3WLNOMdCRtgDDZFajtqhxZxjRg9m0UpIk7HqMAh2gMeyGdcUGT6PcjvT0QdNmDYABwlNygVyTw1IANVW2KhJIGIHviBBTyLiQErLOTraYVeIHPPcyv9QwDJazQUHV0SaGueftUVqpeg4D7RAVLn9nfsUQ37kOFVxjDxKz");
            request.AlwaysMultipartFormData = true;
            request.AddParameter("version", "2.0");
            request.AddParameter("appId", NO_SQL_CONFIG.NHANH_AppId);
            request.AddParameter("businessId", NO_SQL_CONFIG.NHANH_BusinessId);
            request.AddParameter("accessToken", NO_SQL_CONFIG.NHANH_Token);
            request.AddParameter("data", data);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);
            var content = response.Content;

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return content;
            }
            if (content != null)
            {
                return content;
            }
            else
            {
                return string.Empty;
            }
        }

    }
}
