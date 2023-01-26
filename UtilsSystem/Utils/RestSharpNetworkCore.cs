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
    public class RestSharpNetworkCore<T> : RestSharpNetworkUltills<T>
    {
        public RestSharpNetworkCore() {
            _logger = new LoggerManager();
        }
        public override async Task<T> SendRestSharpServicesAsync(string url, RestRequest request, API_LOG_TYPE logServies = API_LOG_TYPE.RECEIVE_LIVE, int requestCode = 0)
        {
            T model = default(T);
            var cancellationTokenSource = new CancellationTokenSource();

            string contentLog = requestCode + "-SendServices " + DateTime.Now.ToString(" dd-MM-yyyy HH:mm:ss.ffff");
            string status = "";
            try
            {
                cancellationTokenSource.CancelAfter(30000); //30s
                contentLog += Environment.NewLine + GetBody(request) + Environment.NewLine;

                var client = new RestClient(url);
                contentLog += url + request.Resource + Environment.NewLine;

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);

                var content = response.Content;

                contentLog += JsonConvert.SerializeObject(response.Content) + " - " + response.StatusCode;
                status = response.StatusCode.ToString();

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return model;
                }

                model = JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception ex)
            {
                await _logger.LogError(requestCode + "- SendAsync()", ex.ToString()).ConfigureAwait(false);
                //_ = new NLogManager().LogError(ex.ToString());
                cancellationTokenSource.Dispose();
                return model;
            }
            finally
            {
                await _logger.LogInfo(requestCode + "- SendAsync()" + DateTime.Now.ToString(" dd-MM-yyyy HH:mm:ss.ffff"), contentLog, status).ConfigureAwait(false);
                //await new NLogManager().LogMessage(contentLog).ConfigureAwait(false);
                cancellationTokenSource.Dispose();
            }
            return model;
        }

        public override async Task<ResponseApiModel<T>> SendRestSharpAsync(string url, RestRequest request, API_LOG_TYPE logServies = API_LOG_TYPE.RECEIVE_LIVE, int requestCode = 0, bool isWriteLog = true)
        {
            ResponseApiModel<T> model = new ResponseApiModel<T>() { Status = EStatusCode.CONNECT_ERROR, Messenger = "", ApiRequestCode = requestCode };
            var cancellationTokenSource = new CancellationTokenSource();
            string contentLog = "";
            string title = requestCode + "-SendAsync " + DateTime.Now.ToString(" dd-MM-yyyy HH:mm:ss.ffff");
            string status = "";
            try
            {
                cancellationTokenSource.CancelAfter(30000); //30s
                contentLog += Environment.NewLine + GetBody(request) + Environment.NewLine;

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
                await _logger.LogError(title, ex.ToString()).ConfigureAwait(false);
                //_ = new NLogManager().LogError(ex.ToString());
                cancellationTokenSource.Dispose();
                return model;
            }
            finally
            {
                if(isWriteLog)
                    await _logger.LogInfo(title, contentLog, status).ConfigureAwait(false);
                cancellationTokenSource.Dispose();
            }
            return model;
        }

        private string GetBody(RestRequest request)
        {
            var bodyParameter = request.Parameters
                .FirstOrDefault(p => p.Type == ParameterType.RequestBody);
            return bodyParameter == null
                ? null
                : bodyParameter.Value.ToString();
        }
    }
}
