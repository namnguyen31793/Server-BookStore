using RestSharp;
using ShareData.API;
using ShareData.DataEnum;
using ShareData.Helper;
using ShareData.LogSystem;
using ShareData.RequestCode;
using ShareData.VtvId.Enum;
using ShareData.VtvId.Model;
using ShareData.VtvId.ResponseCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UtilsSystem.Utils;

namespace ServerEventTet2023.Instance
{
    public class ConnectEventStoreInstance
    {
        private static object _syncObject = new object();
        private static ConnectEventStoreInstance _inst { get; set; }
        public static ConnectEventStoreInstance Inst
        {
            get
            {
                if (_inst == null)
                    lock (_syncObject)
                    {
                        if (_inst == null) _inst = new ConnectEventStoreInstance();
                    }
                return _inst;
            }
        }

        public async Task<ResponseApiLauncher<string>> SendRequestBuyGiftCodeByPiece(long accountId, long transId, long ProductId, long numPiece,string Description)
        {
            ResponseApiLauncher<string> model = new ResponseApiLauncher<string>() { Status = -99, Messenger = "" };

            var request = new RestRequest(ApiVtvIdConfig.Function_Game_Event_Shop_Buy_Giftcode_ByPiece, Method.Post);

            request.AddHeader("Content-Type", "application/json");

            request.AddJsonBody(new
            {
                eventId = ApiVtvIdConfig.Shop_Event_id,
                transactionId = transId,
                accountId = accountId,
                packageId = ProductId,
                quantity = numPiece,
                description = Description
            });

            var responseData = await new RestSharpNetworkCore<ResponseVtvIdServices<ResultGameEventExchangeModel>>().SendRestSharpAsync(ApiVtvIdConfig.ApiVtvId_Game_Event_uri, request, API_LOG_TYPE.RECEIVE_API, REQUEST_API_CODE.API_GAME_EVENT_SHOP_BY_GIFTCODE);

            if (responseData.Status == (int)HttpStatusCode.OK)
            {
                model.Status = responseData.DataResponse.code;
                model.Messenger = responseData.DataResponse.message;
            }
            else
            {
                model.Status = responseData.Status;
                model.Messenger = (int)REQUEST_API_CODE.API_GAME_EVENT_SHOP_BY_GIFTCODE + " " + responseData.Messenger;
            }
            return model;
        }

        public async Task<ResponseApiLauncher<ResultGameEventHistoryExchangeModel[]>> SendRequestGetHistory(long accountId, string startTime, string endTime)
        {
            ResponseApiLauncher<ResultGameEventHistoryExchangeModel[]> model = new ResponseApiLauncher<ResultGameEventHistoryExchangeModel[]>() { Status = -99, Messenger = "" };

            var request = new RestRequest(ApiVtvIdConfig.Function_Game_Event_Shop_History_Giftcode, Method.Post);

            request.AddHeader("Content-Type", "application/json");

            request.AddJsonBody(new
            {
                accountId = accountId,
                beginTime = startTime,
                endTime = endTime
            });

            var responseData = await new RestSharpNetworkCore<ResponseVtvIdServices<ResultGameEventHistoryExchangeModel[]>>().SendRestSharpAsync(ApiVtvIdConfig.ApiVtvId_Game_Event_uri, request, API_LOG_TYPE.RECEIVE_API, REQUEST_API_CODE.API_GAME_EVENT_SHOP_HISTORY);

            if (responseData.Status == (int)HttpStatusCode.OK)
            {
                model.Status = responseData.DataResponse.code;
                model.Messenger = responseData.DataResponse.message;
                if (responseData.DataResponse.code == (int)ERROR_CODE_GAME_EVENT.SUCCESS)
                {
                    model.DataResponse = responseData.DataResponse.data;
                }
            }
            else
            {
                model.Status = responseData.Status;
                model.Messenger = (int)REQUEST_API_CODE.API_GAME_EVENT_SHOP_HISTORY + " " + responseData.Messenger;
            }
            return model;
        }

        public async Task<ResponseApiLauncher<string>> SendRequestBuyOnGByPiece(long accountId, long transId, long numPiece, string Description)
        {
            ResponseApiLauncher<string> model = new ResponseApiLauncher<string>() { Status = -99, Messenger = "" };

            var request = new RestRequest(ApiVtvIdConfig.Function_Game_Event_Shop_Buy_OnG_ByPiece, Method.Post);

            request.AddHeader("Content-Type", "application/json");

            request.AddJsonBody(new
            {
                eventId = ApiVtvIdConfig.Shop_Event_id,
                transactionId = transId,
                accountId = accountId,
                quantity = numPiece,
                description = Description,
            });

            var responseData = await new RestSharpNetworkCore<ResponseVtvIdServices<ResultGameEventExchangeModel>>().SendRestSharpAsync(ApiVtvIdConfig.ApiVtvId_Game_Event_uri, request, API_LOG_TYPE.RECEIVE_API, REQUEST_API_CODE.API_GAME_EVENT_SHOP_BY_ONG);

            if (responseData.Status == (int)HttpStatusCode.OK)
            {
                model.Status = responseData.DataResponse.code;
                model.Messenger = responseData.DataResponse.message;
            }
            else
            {
                model.Status = responseData.Status;
                model.Messenger = (int)REQUEST_API_CODE.API_GAME_EVENT_SHOP_BY_ONG + " " + responseData.Messenger;
            }
            return model;
        }
    }
}
