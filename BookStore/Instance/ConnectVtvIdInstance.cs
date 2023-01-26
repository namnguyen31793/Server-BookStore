using Newtonsoft.Json;
using RestSharp;
using ShareData.API;
using ShareData.DataEnum;
using ShareData.ErrorCode;
using ShareData.Helper;
using ShareData.LogSystem;
using ShareData.RequestCode;
using ShareData.VtvId.ResponseCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UtilsSystem.Utils;
using ServerEventTet2023.Utils;

namespace ServerEventTet2023.Instance
{
    public class ConnectVtvIdInstance
    {

        private static object _syncObject = new object();
        private static ConnectVtvIdInstance _inst { get; set; }
        public static ConnectVtvIdInstance Inst
        {
            get
            {
                if (_inst == null)
                    lock (_syncObject)
                    {
                        if (_inst == null) _inst = new ConnectVtvIdInstance();
                    }
                return _inst;
            }
        }

        public async Task<ResponseApiModel<TokenInfo>> RegisVtvId(string jwtToken)
        {
            ResponseApiModel<TokenInfo> model = new ResponseApiModel<TokenInfo>() { Status = -99, Messenger = "", ApiRequestCode = REQUEST_API_CODE.API_REGIS };

            var request = new RestRequest(ApiVtvIdConfig.Function_Regis, Method.Post);

            request.AddHeader("alg", "HS256");
            request.AddHeader("typ", "JWT");

            //var header = new Dictionary<string, object>
            //{
            //    { "alg", "HS256" },
            //    { "typ", "JWT" },
            //};
            //Guid myuuid = Guid.NewGuid();

            //string uuid = myuuid.ToString();
            //var nbf = (int)(DateTime.UtcNow.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
            //var payload = PayloadModel<RequestRegistv>.GetPayloadModel(uuid, ApiVtvIdConfig.client_Id, nbf, nbf + ApiVtvIdConfig.TOKEN_EXPIRE,
            //    new RequestRegistv()
            //    {
            //        dvId = clientInfo.DeviceId,
            //        os = UltilsHelper.GetOsByType(clientInfo.OsType),
            //        ip = clientInfo.TrueClientIp,
            //        username = e_username,
            //        password = e_password,
            //        email = e_email,
            //        mobile = e_mobile
            //    });
            //byte[] headerBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(header, Formatting.None));
            //byte[] payloadBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload, Formatting.None));

            //var token = Security.Base64UrlEncode(headerBytes) + "." + Security.Base64UrlEncode(payloadBytes);

            //var signature = Security.CreateToken(token, ApiVtvIdConfig.client_secret);
            //var jwtToken = token + "." + signature;

            request.AddJsonBody(new
            {
                jwt = jwtToken,
            });

            var responseData = await new RestSharpNetworkCore<ResponseVtvIdServices<TokenInfo>>().SendRestSharpAsync(ApiVtvIdConfig.ApiVtvId_Authen_uri, request, API_LOG_TYPE.RECEIVE_API, REQUEST_API_CODE.API_REGIS);

            if (responseData.Status == (int)HttpStatusCode.OK)
            {
                model.Status = responseData.DataResponse.code;
                model.Messenger = responseData.DataResponse.message;
            }
            else
            {
                model.Status = responseData.Status;
                model.Messenger = (int)REQUEST_API_CODE.API_REGIS + " " + responseData.Messenger;
            }
            return model;
        }

        public async Task<ResponseApiModel<TokenInfo>> LoginVtvId(string jwtToken)//(RequestLogin data
        {
            ResponseApiModel<TokenInfo> model = new ResponseApiModel<TokenInfo>() { Status = -99, Messenger = "", ApiRequestCode = REQUEST_API_CODE.API_LOGIN };
            if (StaticData.IsInMaintain)
            {
                if (UltilsHelper.IsNotMyIP())
                {
                    model.Status = EStatusCode.SYSTEM_MAINTAIN;
                    model.Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_MAINTAIN);
                    return model;
                }
            }

            var request = new RestRequest(ApiVtvIdConfig.Function_Login, Method.Post);

            request.AddHeader("alg", "HS256");
            request.AddHeader("typ", "JWT");
            //var header = new Dictionary<string, object>
            //{
            //    { "alg", "HS256" },
            //    { "typ", "JWT" },
            //};
            //Guid myuuid = Guid.NewGuid();

            //string uuid = myuuid.ToString();
            //var nbf = (int)(DateTime.UtcNow.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
            //var payload = PayloadModel<RequestLoginVtv>.GetPayloadModel(uuid, ApiVtvIdConfig.client_Id, nbf, nbf + ApiVtvIdConfig.TOKEN_EXPIRE,
            //    new RequestLoginVtv() {
            //        dvId = clientInfo.DeviceId,
            //        os = UltilsHelper.GetOsByType(clientInfo.OsType),
            //        ip = clientInfo.TrueClientIp,
            //        username = e_username,
            //        Passwordmd5 = Security.MD5Encrypt(e_password)
            //    });
            //byte[] headerBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(header, Formatting.None));
            //byte[] payloadBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload, Formatting.None));

            //var token = Security.Base64UrlEncode(headerBytes)+ "."+ Security.Base64UrlEncode(payloadBytes);

            //var signature = Security.CreateToken(token, ApiVtvIdConfig.client_secret);
            //var jwtToken = token + "." + signature;

            request.AddJsonBody(new
            {
                jwt = jwtToken,
            });

            var responseData = await new RestSharpNetworkCore<ResponseVtvIdServices<TokenInfo>>().SendRestSharpAsync(ApiVtvIdConfig.ApiVtvId_Authen_uri, request, API_LOG_TYPE.RECEIVE_API, REQUEST_API_CODE.API_LOGIN);

            if (responseData.Status == (int)HttpStatusCode.OK)
            {
                model.Status = responseData.DataResponse.code;
                model.Messenger = responseData.DataResponse.message;
                if (responseData.DataResponse.code == 0)
                {
                    model.DataResponse = responseData.DataResponse.data;
                }
            }
            else
            {
                model.Status = responseData.Status;
                model.Messenger = (int)REQUEST_API_CODE.API_LOGIN + " " + responseData.Messenger;
            }
            return model;
        }

        public async Task<ResponseApiModel<TokenInfo>> LoginFbVtvId(string jwtToken)//(RequestLogin data
        {
            ResponseApiModel<TokenInfo> model = new ResponseApiModel<TokenInfo>() { Status = -99, Messenger = "", ApiRequestCode = REQUEST_API_CODE.API_LOGIN_FB };
            if (StaticData.IsInMaintain)
            {
                if (UltilsHelper.IsNotMyIP())
                {
                    model.Status = EStatusCode.SYSTEM_MAINTAIN;
                    model.Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_MAINTAIN);
                    return model;
                }
            }

            var request = new RestRequest(ApiVtvIdConfig.Function_Login_Fb, Method.Post);

            request.AddHeader("alg", "HS256");
            request.AddHeader("typ", "JWT");

            //var header = new Dictionary<string, object>
            //{
            //    { "alg", "HS256" },
            //    { "typ", "JWT" },
            //};
            //Guid myuuid = Guid.NewGuid();

            //string uuid = myuuid.ToString();
            //var nbf = (int)(DateTime.UtcNow.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
            //var payload = PayloadModel<RequestLoginFbVtv>.GetPayloadModel(uuid, ApiVtvIdConfig.client_Id, nbf, nbf + ApiVtvIdConfig.TOKEN_EXPIRE,
            //    new RequestLoginFbVtv()
            //    {
            //        dvId = clientInfo.DeviceId,
            //        os = UltilsHelper.GetOsByType(clientInfo.OsType),
            //        ip = clientInfo.TrueClientIp,
            //        fbId = fbId,
            //        fbEmail = fbEmail,
            //        fbName = fbName,
            //        fbToken = fbToken,
            //        businessToken = businessToken,
            //    });
            //byte[] headerBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(header, Formatting.None));
            //byte[] payloadBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload, Formatting.None));

            //var token = Security.Base64UrlEncode(headerBytes) + "." + Security.Base64UrlEncode(payloadBytes);

            //var signature = Security.CreateToken(token, ApiVtvIdConfig.client_secret);
            //var jwtToken = token + "." + signature;

            request.AddJsonBody(new
            {
                jwt = jwtToken,
            });

            var responseData = await new RestSharpNetworkCore<ResponseVtvIdServices<TokenInfo>>().SendRestSharpAsync(ApiVtvIdConfig.ApiVtvId_Authen_uri, request, API_LOG_TYPE.RECEIVE_API, REQUEST_API_CODE.API_LOGIN_FB);

            if (responseData.Status == (int)HttpStatusCode.OK)
            {
                model.Status = responseData.DataResponse.code;
                model.Messenger = responseData.DataResponse.message;
                if (responseData.DataResponse.code == 0)
                {
                    model.DataResponse = responseData.DataResponse.data;
                }
            }
            else
            {
                model.Status = responseData.Status;
                model.Messenger = (int)REQUEST_API_CODE.API_LOGIN_FB + " " + responseData.Messenger;
            }
            return model;
        }

        public async Task<ResponseApiModel<TokenInfo>> LoginGgVtvId(string jwtToken)//(RequestLogin data
        {
            ResponseApiModel<TokenInfo> model = new ResponseApiModel<TokenInfo>() { Status = -99, Messenger = "", ApiRequestCode = REQUEST_API_CODE.API_LOGIN_GG };
            if (StaticData.IsInMaintain)
            {
                if (UltilsHelper.IsNotMyIP())
                {
                    model.Status = EStatusCode.SYSTEM_MAINTAIN;
                    model.Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_MAINTAIN);
                    return model;
                }
            }

            var request = new RestRequest(ApiVtvIdConfig.Function_Login_Gg, Method.Post);

            request.AddHeader("alg", "HS256");
            request.AddHeader("typ", "JWT");

            //var header = new Dictionary<string, object>
            //{
            //    { "alg", "HS256" },
            //    { "typ", "JWT" },
            //};
            //Guid myuuid = Guid.NewGuid();

            //string uuid = myuuid.ToString();
            //var nbf = (int)(DateTime.UtcNow.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
            //var payload = PayloadModel<RequestLoginGGVtv>.GetPayloadModel(uuid, ApiVtvIdConfig.client_Id, nbf, nbf + ApiVtvIdConfig.TOKEN_EXPIRE,
            //    new RequestLoginGGVtv()
            //    {
            //        dvId = clientInfo.DeviceId,
            //        os = UltilsHelper.GetOsByType(clientInfo.OsType),
            //        ip = clientInfo.TrueClientIp,
            //        ggId = ggId,
            //        ggEmail = ggEmail,
            //        ggName = ggName,
            //        code = ggCode,
            //    });
            //byte[] headerBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(header, Formatting.None));
            //byte[] payloadBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload, Formatting.None));

            //var token = Security.Base64UrlEncode(headerBytes) + "." + Security.Base64UrlEncode(payloadBytes);

            //var signature = Security.CreateToken(token, ApiVtvIdConfig.client_secret);
            //var jwtToken = token + "." + signature;

            request.AddJsonBody(new
            {
                jwt = jwtToken,
            });

            var responseData = await new RestSharpNetworkCore<ResponseVtvIdServices<TokenInfo>>().SendRestSharpAsync(ApiVtvIdConfig.ApiVtvId_Authen_uri, request, API_LOG_TYPE.RECEIVE_API, REQUEST_API_CODE.API_LOGIN_GG);

            if (responseData.Status == (int)HttpStatusCode.OK)
            {
                model.Status = responseData.DataResponse.code;
                model.Messenger = responseData.DataResponse.message;
                if (responseData.DataResponse.code == 0)
                {
                    model.DataResponse = responseData.DataResponse.data;
                }
            }
            else
            {
                model.Status = responseData.Status;
                model.Messenger = (int)REQUEST_API_CODE.API_LOGIN_GG + " " + responseData.Messenger;
            }
            return model;
        }

        public async Task<ResponseApiModel<TokenInfo>> RefreshTokenVtvId(string refresh_token, string DeviceId, EOSType OsType, string TrueClientIp)
        {
            ResponseApiModel<TokenInfo> model = new ResponseApiModel<TokenInfo>() { Status = -99, Messenger = "", ApiRequestCode = REQUEST_API_CODE.API_REFRESH_TOKEN_VTV };

            var request = new RestRequest(ApiVtvIdConfig.Function_Refresh_Token, Method.Post);

            request.AddHeader("Content-Type", "application/json");

            request.AddJsonBody(new
            {
                jwt = refresh_token,
                cid = ApiVtvIdConfig.client_Id,
                dvId = DeviceId,
                os = UltilsHelper.GetOsByType(OsType),
                ip = TrueClientIp
            });

            var responseData = await new RestSharpNetworkCore<ResponseVtvIdServices<TokenInfo>>().SendRestSharpAsync(ApiVtvIdConfig.ApiVtvId_Authen_uri, request, API_LOG_TYPE.RECEIVE_API, REQUEST_API_CODE.API_REFRESH_TOKEN_VTV, false);

            if (responseData.Status == (int)HttpStatusCode.OK)
            {
                model.Status = responseData.DataResponse.code;
                model.Messenger = responseData.DataResponse.message;
                if (responseData.DataResponse.code == 0)
                {
                    model.DataResponse = responseData.DataResponse.data;
                    //save new info with token
                    //await GetUserInfoVtvId(model.DataResponse.access_token, DeviceId, OsType, TrueClientIp).ConfigureAwait(false);
                }
            }
            else
            {
                model.Status = responseData.Status;
                model.Messenger = (int)REQUEST_API_CODE.API_REFRESH_TOKEN_VTV + " " + responseData.Messenger;
            }
            return model;
        }


        public async Task<ResponseApiModel<GoProfile>> GetUserInfoVtvId(string access_token, string DeviceId, EOSType OsType, string TrueClientIp)
        {
            ResponseApiModel<GoProfile> model = new ResponseApiModel<GoProfile>() { Status = -99, Messenger = "", ApiRequestCode = REQUEST_API_CODE.API_GET_ACTOR_INFO_VTV };

            var request = new RestRequest(ApiVtvIdConfig.Function_Get_User_Info, Method.Post);

            request.AddHeader("Content-Type", "application/json");

            request.AddJsonBody(new
            {
                jwt = access_token,
                cid = ApiVtvIdConfig.client_Id,
                dvId = DeviceId,
                os = UltilsHelper.GetOsByType(OsType),
                ip = TrueClientIp,
            });

            var responseData = await new RestSharpNetworkCore<ResponseVtvIdServices<GoProfile>>().SendRestSharpAsync(ApiVtvIdConfig.ApiVtvId_Userinfo_uri, request, API_LOG_TYPE.RECEIVE_API, REQUEST_API_CODE.API_GET_ACTOR_INFO_VTV);

            if (responseData.Status == (int)HttpStatusCode.OK)
            {
                model.Status = responseData.DataResponse.code;
                model.Messenger = responseData.DataResponse.message;
                if (responseData.DataResponse.code == 0)
                {
                    model.DataResponse = responseData.DataResponse.data;
                    //Tracking_Account_Instance.Inst.Tracking_Account_Login(responseData.DataResponse.data.AccountID.ToString(), (int)OsType);
                    new TokenUtils().AddTokenCache(access_token, JsonConvert.SerializeObject(model.DataResponse), 180);
                    //new TokenUtils().AddTokenCache(access_token, MessagePackSerializer.Serialize(model.DataResponse), 180);
                }
            }
            else
            {
                model.Status = responseData.Status;
                model.Messenger = (int)REQUEST_API_CODE.API_GET_ACTOR_INFO_VTV + " " + responseData.Messenger;
            }

            return model;
        }

        public async Task<ResponseApiModel<GoBalance>> GetUserBalanceVtvId(string access_token, string userId, ClientRequestInfo clientInfo)
        {
            ResponseApiModel<GoBalance> model = new ResponseApiModel<GoBalance>() { Status = -99, Messenger = "", ApiRequestCode = REQUEST_API_CODE.API_GET_ACTOR_BALANCE_VTV };

            var request = new RestRequest(ApiVtvIdConfig.Function_Get_User_Balance, Method.Post);

            request.AddHeader("Content-Type", "application/json");

            request.AddJsonBody(new
            {
                jwt = access_token,
                cid = ApiVtvIdConfig.client_Id,
                dvId = clientInfo.DeviceId,
                os = UltilsHelper.GetOsByType(clientInfo.OsType),
                ip = clientInfo.TrueClientIp,
                mty = 10,
                userId = long.Parse(userId)
            });

            var responseData = await new RestSharpNetworkCore<ResponseVtvIdServices<GoBalance>>().SendRestSharpAsync(ApiVtvIdConfig.ApiVtvId_Userinfo_uri, request, API_LOG_TYPE.RECEIVE_API, REQUEST_API_CODE.API_GET_ACTOR_BALANCE_VTV);

            if (responseData.Status == (int)HttpStatusCode.OK)
            {
                model.Status = responseData.DataResponse.code;
                model.Messenger = responseData.DataResponse.message;
                if (responseData.DataResponse.code == 0)
                {
                    model.DataResponse = responseData.DataResponse.data;
                }
            }
            else
            {
                model.Status = responseData.Status;
                model.Messenger = (int)REQUEST_API_CODE.API_GET_ACTOR_BALANCE_VTV + " " + responseData.Messenger;
            }

            //if (responseData != null)
            //{
            //    model.Status = responseData.code;
            //    model.Messenger = responseData.message;
            //    if (responseData.code == 0)
            //    {
            //        model.DataResponse = responseData.data;
            //    }
            //}
            //else
            //{
            //    model.Status = EStatusCode.CONNECT_ERROR;
            //    model.Messenger = REQUEST_API_CODE.API_GET_ACTOR_BALANCE_VTV + " " + UltilsHelper.GetMessageByErrorCode(EStatusCode.CONNECT_ERROR);
            //}
            return model;
        }
    }
}
