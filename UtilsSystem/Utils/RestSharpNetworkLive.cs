using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using ShareData.LogSystem;
using Newtonsoft.Json;
using System.Net;
using System.Threading;
using ShareData.API;
using ShareData.ErrorCode;
using ShareData.VtvId.ResponseCode;
using ShareData.Config.Model;
using LoggerService;

namespace UtilsSystem.Utils
{
    public class RestSharpNetworkLive<T> : RestSharpNetworkUltills<T>
    {
        public RestSharpNetworkLive()
        {
            _logger = new LoggerManager();
        }

        public override async Task<T> SendRestSharpServicesAsync(string url, RestRequest request, API_LOG_TYPE logServies = API_LOG_TYPE.RECEIVE_LIVE, int requestCode = 0) {

            T model = default(T);
            var cancellationTokenSource = new CancellationTokenSource();

            string contentLog = null;// requestCode + "-SendRestSharpLiveServices " + DateTime.Now.ToString(" dd-MM-yyyy HH:mm:ss.ffff");
            string status = "";
            try
            {
                cancellationTokenSource.CancelAfter(30000); //30s
                contentLog += Environment.NewLine + JsonConvert.SerializeObject(request.Parameters) + Environment.NewLine;

                var client = new RestClient(url);
                contentLog += url + request.Resource + Environment.NewLine;

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);

                var content = response.Content;

                contentLog += JsonConvert.SerializeObject(response.Content) + " - " + response.StatusCode;
                status = response.StatusCode.ToString();
                //code = (int)response.StatusCode;
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    //code = (int)response.StatusCode;
                    //return model;
                }
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return model;
                }

                model = JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception ex)
            {
                await _logger.LogError(requestCode + "- SendRestSharpLiveServicesAsync()", ex.ToString()).ConfigureAwait(false);
                //_ = new NLogManager().LogError(ex.ToString());
                cancellationTokenSource.Dispose();
                return model;
            }
            finally
            {
                await _logger.LogInfo(requestCode + "- SendRestSharpLiveAsync()" + DateTime.Now.ToString(" dd-MM-yyyy HH:mm:ss.ffff"), contentLog, status).ConfigureAwait(false);
                //await new NLogManager().LogMessage(contentLog).ConfigureAwait(false);
                cancellationTokenSource.Dispose();
            }
            return model;
        }

        public override async Task<ResponseApiModel<T>> SendRestSharpAsync(string url, RestRequest request, API_LOG_TYPE logServies = API_LOG_TYPE.RECEIVE_LIVE, int requestCode = 0, bool isWriteLog = true)
        {
            ResponseApiModel<T> model = new ResponseApiModel<T>() { Status = EStatusCode.CONNECT_ERROR, Messenger = "", ApiRequestCode = requestCode };
            var cancellationTokenSource = new CancellationTokenSource();

            string contentLog = null;// requestCode + "-SendRestSharpLiveAsync " + DateTime.Now.ToString(" dd-MM-yyyy HH:mm:ss.ffff");
            string status = "";
            try
            {
                cancellationTokenSource.CancelAfter(30000); //30s
                contentLog += Environment.NewLine + JsonConvert.SerializeObject(request.Parameters) + Environment.NewLine;

                var client = new RestClient(url);
                contentLog += url + request.Resource + Environment.NewLine;

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);

                var content = response.Content;
                contentLog += JsonConvert.SerializeObject(response.Content) + " - " + response.StatusCode;
                status = response.StatusCode.ToString();

                model.Status = (int)response.StatusCode;
                model.Messenger = response.StatusCode.ToString();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return model;
                }
                model.DataResponse = JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception ex)
            {
                await _logger.LogError(requestCode + "- SendRestSharpLiveAsync()", ex.ToString()).ConfigureAwait(false);
                //_ = new NLogManager().LogError(ex.ToString());
                cancellationTokenSource.Dispose();
                return model;
            }
            finally
            {
                await _logger.LogInfo(requestCode + "- SendRestSharpLiveAsync()" + DateTime.Now.ToString(" dd-MM-yyyy HH:mm:ss.ffff"), contentLog, status).ConfigureAwait(false);
                //if(isWriteLog)
                //    await new NLogManager().LogMessage(contentLog).ConfigureAwait(false);
                cancellationTokenSource.Dispose();
            }
            return model;
        }
    }
}
