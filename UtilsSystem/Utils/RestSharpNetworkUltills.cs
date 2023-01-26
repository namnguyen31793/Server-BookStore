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
using LoggerService;

namespace UtilsSystem.Utils
{
    public class RestSharpNetworkUltills<T>
    {
        private protected ILoggerManager _logger;
        public RestSharpNetworkUltills(){
            _logger = new LoggerManager();
        }

        public virtual async Task<T> SendRestSharpServicesAsync(string url, RestRequest request, API_LOG_TYPE logServies = API_LOG_TYPE.RECEIVE_LIVE, int requestCode = 0)
        {
            T model = default(T);
            var cancellationTokenSource = new CancellationTokenSource();

            try
            {
                cancellationTokenSource.CancelAfter(30000); //30s

                var client = new RestClient(url);

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);

                var content = response.Content;

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
                await _logger.LogError(requestCode + "- SendRestSharpServicesAsync()", ex.ToString()).ConfigureAwait(false);
                //_ = new NLogManager().LogError(ex.ToString());
                cancellationTokenSource.Dispose();
                return model;
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
            return model;
        }

        public virtual async Task<ResponseApiModel<T>> SendRestSharpAsync(string url, RestRequest request, API_LOG_TYPE logServies = API_LOG_TYPE.RECEIVE_LIVE, int requestCode = 0, bool isWriteLog = true)
        {
            ResponseApiModel<T> model = new ResponseApiModel<T>() { Status = EStatusCode.CONNECT_ERROR, Messenger = "", ApiRequestCode = requestCode };
            var cancellationTokenSource = new CancellationTokenSource();

            string contentLog = requestCode + "-SendRestSharpCoreAsync " + DateTime.Now.ToString(" dd-MM-yyyy HH:mm:ss.ffff");
            string status = "";
            try
            {
                cancellationTokenSource.CancelAfter(30000); //30s
                contentLog += Environment.NewLine + JsonConvert.SerializeObject(request.Parameters) + Environment.NewLine;

                var client = new RestClient(url);
                contentLog += url + request.Resource + Environment.NewLine;

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);
                contentLog += JsonConvert.SerializeObject(response.Content) + " - " + response.StatusCode;
                status = response.StatusCode.ToString();

                var content = response.Content;

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
                await _logger.LogError(requestCode + "- SendRestSharpCoreAsync()", ex.ToString()).ConfigureAwait(false);
                //_ = new NLogManager().LogError(ex.ToString());
                cancellationTokenSource.Dispose();
                return model;
            }
            finally
            {
                //LogInstance.Inst.LogNormal(logServies, contentLog, title);
                if (isWriteLog)
                    await _logger.LogInfo(requestCode + "- RestSharpNetworkCoreAsync()" + DateTime.Now.ToString(" dd-MM-yyyy HH:mm:ss.ffff"), contentLog, status).ConfigureAwait(false);
                cancellationTokenSource.Dispose();
            }
            return model;
        }

        public static T SendRestSharp(string url, RestRequest request, out int code, API_LOG_TYPE logServies = API_LOG_TYPE.RECEIVE_LIVE, int requestCode = 0)
        {
            T model = default(T);
            code = EStatusCode.CONNECT_ERROR;

            try
            {
                var client = new RestClient(url);

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                var response = client.Execute(request);

                var content = response.Content;

                code = (int)response.StatusCode;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return model;
                }

                model = JsonConvert.DeserializeObject<T>(content);
                return model;
            }
            catch (Exception)
            {
                //_ = new NLogManager().LogError(ex.ToString());
                return model;
            }
        }
    }
}
