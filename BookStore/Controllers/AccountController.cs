using BookStore.Instance;
using BookStore.Utils;
using DAO.DAOImp;
using LoggerService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedisSystem;
using ShareData.API;
using ShareData.DB.Users;
using ShareData.ErrorCode;
using ShareData.Request;
using ShareData.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UtilsSystem.SocialNetwork;
using UtilsSystem.Utils;

namespace BookStore.Controllers
{
    [ApiVersion("1.0")]
    [Route("v1/Accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        
        private ILoggerManager _logger;
        public AccountController(ILoggerManager logger)
        {
            _logger = logger;
        }
        private string token = string.Empty;

        [HttpPost]
        [Route("Login")]
        [ResponseCache(Duration = 5)]
        public async Task<IActionResult> Login(RequestAuthenModel requestLogin)
        {
            var response = new ResponseApiModel<TokenInfo>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            if (!AccountUtils.IsLoginRequestTrue(requestLogin))
            {
                return Ok(new ResponseApiLauncher<string>() { Status = EStatusCode.DATA_INVAILD, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.DATA_INVAILD) });
            }
            try
            {
                var clientInfo = new ClientRequestInfo(Request);
                response = await LoginAsync(requestLogin, clientInfo);
            }
            catch (Exception ex)
            {
                await _logger.LogError("Account-Login{}", ex.ToString()).ConfigureAwait(false);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("LoginFacebook")]
        public async Task<IActionResult> LoginFacebook(RequestAuthenSocial requestLogin)
        {
            if (!AccountUtils.IsLoginRequestTrue(requestLogin)){
                return Ok(new ResponseApiLauncher<string>() { Status = EStatusCode.DATA_INVAILD, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.DATA_INVAILD) });
            }
            if(!string.IsNullOrEmpty(requestLogin.Email))
                if(!AccountUtils.IsValidEmail(requestLogin.Email)) 
                    return Ok(new ResponseApiLauncher<string>() { Status = EStatusCode.EMAIL_INVAILD, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.EMAIL_INVAILD) });
            if (!string.IsNullOrEmpty(requestLogin.PhoneNumber))
                if (!AccountUtils.IsPhoneNumber(requestLogin.PhoneNumber))
                    return Ok(new ResponseApiLauncher<string>() { Status = EStatusCode.PHONE_INVAILD, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.PHONE_INVAILD) });

            var clientInfo = new ClientRequestInfo(Request);

            var facebookUserName = await FacebookHelper.GetFacebookUserNameAsync(requestLogin.Token);
            await _logger.LogError("Account-Register{}", facebookUserName).ConfigureAwait(false);
            var responseCode = -99;
            if (string.IsNullOrEmpty(facebookUserName))
                return Ok(new ResponseApiLauncher<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) });
            var fbPassword = FacebookHelper.GetFacebookPassword(facebookUserName);
            var passMd5 = AccountUtils.EncryptPasswordMd5(fbPassword);
            responseCode = await Task.Run(() =>
            {
                int accountId = 0;
                var res = StoreUsersDAO.Inst.Register(REGISTER_TYPE.FACEBOOK_TYPE, facebookUserName, "", passMd5, clientInfo.MerchantId, clientInfo.TrueClientIp, clientInfo.DeviceId, (int)clientInfo.OsType,
                    requestLogin.PhoneNumber, requestLogin.Email, out accountId);
                return res;
            
            });
            if (responseCode == (int)EStatusCode.SUCCESS)
            {
                //TrackingDAO.Inst.TrackingRegis();
            }
            if (responseCode == (int)EStatusCode.ACCOUNT_EXITS || responseCode == (int)EStatusCode.SUCCESS)
            {
                //do login
                RequestAuthenModel request = new RequestAuthenModel()
                {
                    AccountName = facebookUserName,
                    Password = fbPassword,
                };
                var response = await LoginAsync(request, clientInfo);
                return Ok(response);
            }
            else
            {
                return Ok(new ResponseApiLauncher<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) });
            }
        }

        [HttpPost]
        [Route("Register")]
        [ResponseCache(Duration = 5)]
        public async Task<IActionResult> Regis(RequestRegisterModel requestRegis)
        {
            if (!AccountUtils.IsRegisterRequestTrue(requestRegis)) {
                return Ok(new ResponseApiLauncher<string>() { Status = EStatusCode.DATA_INVAILD, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.DATA_INVAILD) });
            }
            if (!string.IsNullOrEmpty(requestRegis.Email))
                if (!AccountUtils.IsValidEmail(requestRegis.Email))
                    return Ok(new ResponseApiLauncher<string>() { Status = EStatusCode.EMAIL_INVAILD, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.EMAIL_INVAILD) });
            if (!string.IsNullOrEmpty(requestRegis.PhoneNumber))
                if (!AccountUtils.IsPhoneNumber(requestRegis.PhoneNumber))
                    return Ok(new ResponseApiLauncher<string>() { Status = EStatusCode.PHONE_INVAILD, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.PHONE_INVAILD) });

            var clientInfo = new ClientRequestInfo(Request);
            var responseCode = -99;
            try
            {
                responseCode = await Task.Run(() =>
                {
                    int accountId = 0;
                    var res = StoreUsersDAO.Inst.Register(REGISTER_TYPE.NORMAL_TYPE, requestRegis.AccountName, "", AccountUtils.EncryptPasswordMd5(requestRegis.Password), clientInfo.MerchantId, clientInfo.TrueClientIp, clientInfo.DeviceId, (int)clientInfo.OsType,
                        requestRegis.PhoneNumber, requestRegis.Email, out accountId);
                    return res;

                });
                if (responseCode == (int)EStatusCode.SUCCESS)
                {
                    //TrackingDAO.Inst.TrackingRegis();
                }
                if (responseCode == (int)EStatusCode.ACCOUNT_EXITS || responseCode == (int)EStatusCode.SUCCESS)
                {
                    //do login
                    RequestAuthenModel request = new RequestAuthenModel()
                    {
                        AccountName = requestRegis.AccountName,
                        Password = requestRegis.Password,
                    };
                    var response = await LoginAsync(request, clientInfo);
                    return Ok(response);
                }
                else
                {
                    return Ok(new ResponseApiLauncher<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) });
                }
            }
            catch (Exception ex)
            {
                await _logger.LogError("Account-Register{}", ex.ToString()).ConfigureAwait(false);
                return Ok(new ResponseApiLauncher<string>() { Status = EStatusCode.SUCCESS, Messenger = UtilsSystem.Utils.UltilsHelper.GetMessageByErrorCode(EStatusCode.SUCCESS) });
            }
        }

        private async Task<ResponseApiModel<TokenInfo>> LoginAsync(RequestAuthenModel requestLogin, ClientRequestInfo clientInfo)
        {
            ResponseApiModel<TokenInfo> model = new ResponseApiModel<TokenInfo>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            try
            {
                var res = EStatusCode.SUCCESS;
                res = await Task.Run(() =>
                {
                    var accountId = StoreUsersDAO.Inst.DoLogin(requestLogin.AccountName, AccountUtils.EncryptPasswordMd5(requestLogin.Password), clientInfo.MerchantId, clientInfo.TrueClientIp, (int)clientInfo.OsType, ref res);
                    if (accountId >= 0)
                    {
                        //create refresh token -> save db
                        var refreshToken = TokenManager.GenerateRefreshToken();
                        var responseToken = StoreUsersDAO.Inst.AddToken(accountId, refreshToken);
                        if (responseToken >= 0)
                        {
                            var accessToken = TokenManager.GenerateAccessToken(accountId, clientInfo);
                            model.DataResponse = new TokenInfo() { AccountId = accountId, Access_token = accessToken, Refresh_token = refreshToken };
                        }
                        else
                        {
                            res = (int)EStatusCode.SYSTEM_ERROR;
                        }
                        //tracking login
                    }
                    else
                    {
                        res = (int)EStatusCode.ACCOUNT_NOT_EXITS;
                    }
                    return res;
                });
                model.Status = res;
                model.Messenger = UltilsHelper.GetMessageByErrorCode(res);
            }
            catch (Exception ex)
            {
                await _logger.LogError("Account-Login{}", ex.ToString()).ConfigureAwait(false);
            }
            return model;
        }

        [HttpGet]
        [Route("RefreshToken")]
        [ResponseCache(Duration = 5)]
        public async Task<IActionResult> RefreshToken(TokenInfo data)
        {
            try
            {
                //check token
                var response = StoreUsersDAO.Inst.CheckRefreshToken(data.Refresh_token);
                if (response >= 0)
                {
                    var clientInfo = new ClientRequestInfo(Request);
                    var accessToken = TokenManager.GenerateAccessToken(data.AccountId, clientInfo);
                    var model = new TokenInfo() { AccountId = data.AccountId, Access_token = accessToken, Refresh_token = data.Refresh_token };
                    return Ok(new ResponseApiModel<TokenInfo>() { Status = EStatusCode.SUCCESS, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SUCCESS), DataResponse = model });
                }
                else if(response == EStatusCode.TOKEN_INVALID)
                {
                    return Ok(new ResponseApiModel<TokenInfo>() { Status = EStatusCode.TOKEN_INVALID, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.TOKEN_INVALID) });
                }
                else if (response == EStatusCode.TOKEN_EXPIRES)
                {
                    return Ok(new ResponseApiModel<TokenInfo>() { Status = EStatusCode.TOKEN_EXPIRES, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.TOKEN_EXPIRES) });
                }
            }
            catch (Exception ex)
            {
                await _logger.LogError("Account-RefreshToken{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(new ResponseApiLauncher<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) });
        }

        [HttpGet]
        [Route("GetAccountInfo")]
        [ResponseCache(Duration = 5)]
        public async Task<IActionResult> GetAccountInfo()
        {
            int responseStatus = -99;
            AccountModel response = null;
            try
            {
                if (Request.Headers.TryGetValue("Authorization", out var values)) {
                    token = values.FirstOrDefault();
                }
                if (string.IsNullOrEmpty(token))
                    return Ok(new ResponseApiModel<string>() { Status = EStatusCode.USER_NOT_LOGIN, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.USER_NOT_LOGIN) });
                long accountId = TokenManager.GetAccountIdByAccessToken(token);
                if ( accountId <= 0)
                    return Ok(new ResponseApiModel<string>() { Status = EStatusCode.TOKEN_EXPIRES, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.TOKEN_EXPIRES) });
                response = StoreUsersDAO.Inst.GetAccountInfo(accountId, ref responseStatus);
            }
            catch (Exception ex)
            {
                await _logger.LogError("Account-GetAccountInfo{}", ex.ToString()).ConfigureAwait(false);
            }

            return Ok(new ResponseApiLauncher<AccountModel>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = response });
        }

        [HttpPost]
        [Route("UpdateEmail")]
        [ResponseCache(Duration = 5)]
        public async Task<IActionResult> UpdateEmail(string Email)
        {
            int responseStatus = -99;
            try
            {
                if (Request.Headers.TryGetValue("Authorization", out var values))
                {
                    token = values.FirstOrDefault();
                }
                if (string.IsNullOrEmpty(token))
                    return Ok(new ResponseApiModel<string>() { Status = EStatusCode.USER_NOT_LOGIN, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.USER_NOT_LOGIN) });
                long accountId = TokenManager.GetAccountIdByAccessToken(token);
                if (accountId <= 0)
                    return Ok(new ResponseApiModel<string>() { Status = EStatusCode.TOKEN_EXPIRES, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.TOKEN_EXPIRES) });
                responseStatus = StoreUsersDAO.Inst.UpdateEmail(accountId, Email);
                if (responseStatus == 0) {
                    //send email
                    //EmailInstance.Inst.SendGmailMail();
                }
            }
            catch (Exception ex)
            {
                await _logger.LogError("Account-UpdateEmail{}", ex.ToString()).ConfigureAwait(false);
            }

            return Ok(new ResponseApiLauncher<AccountModel>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus)});
        }
        [HttpPost]
        [Route("UpdateInfo")]
        [ResponseCache(Duration = 5)]
        public async Task<IActionResult> UpdateInfo(RequestUpdateInfoModel model)
        {
            int responseStatus = -99;
            try
            {
                if (Request.Headers.TryGetValue("Authorization", out var values))
                {
                    token = values.FirstOrDefault();
                }
                if (string.IsNullOrEmpty(token))
                    return Ok(new ResponseApiModel<string>() { Status = EStatusCode.USER_NOT_LOGIN, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.USER_NOT_LOGIN) });
                long accountId = TokenManager.GetAccountIdByAccessToken(token);
                if (accountId <= 0)
                    return Ok(new ResponseApiModel<string>() { Status = EStatusCode.TOKEN_EXPIRES, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.TOKEN_EXPIRES) });
                responseStatus = StoreUsersDAO.Inst.UpdateInfo(accountId, model.Nickname, model.AvatarId, model.PhoneNumber, model.BirthDay, model.Adress);
            }
            catch (Exception ex)
            {
                await _logger.LogError("Account-UpdateInfo{}", ex.ToString()).ConfigureAwait(false);
            }

            return Ok(new ResponseApiLauncher<AccountModel>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus) });
        }

    }
}
