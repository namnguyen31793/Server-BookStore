﻿using BookStore.Instance;
using BookStore.Interfaces;
using BookStore.Utils;
using DAO.DAOImp;
using LoggerService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
using System.Web;
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
        private IEmailSender _emailSender;
        private ITokenManager _tokenManager;
        public AccountController(ILoggerManager logger, IEmailSender emailSender, ITokenManager tokenManager)
        {
            _logger = logger;
            _emailSender = emailSender;
            _tokenManager = tokenManager;
        }
        private string token = string.Empty;

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(RequestAuthenModel requestLogin)
        {
            var response = new ResponseApiModel<TokenInfo>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            if (!AccountUtils.IsLoginRequestTrue(requestLogin))
            {
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.DATA_INVAILD, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.DATA_INVAILD) });
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
        [Route("LoginGoogle")]
        public async Task<IActionResult> LoginGoogle(RequestAuthenSocial requestLogin)
        {
            if (!AccountUtils.IsLoginRequestTrue(requestLogin))
            {
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.DATA_INVAILD, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.DATA_INVAILD) });
            }
            if (!string.IsNullOrEmpty(requestLogin.Email))
                if (!AccountUtils.IsValidEmail(requestLogin.Email))
                    return Ok(new ResponseApiModel<string>() { Status = EStatusCode.EMAIL_INVAILD, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.EMAIL_INVAILD) });
            if (!string.IsNullOrEmpty(requestLogin.PhoneNumber))
                if (!AccountUtils.IsPhoneNumber(requestLogin.PhoneNumber))
                    return Ok(new ResponseApiModel<string>() { Status = EStatusCode.PHONE_INVAILD, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.PHONE_INVAILD) });
            try
            {
                var clientInfo = new ClientRequestInfo(Request);

                var facebookUserName = await GoogleHelper.GetGoogleUserNameAsync(requestLogin.Token);
                await _logger.LogError("Account-Register{}", facebookUserName).ConfigureAwait(false);
                var responseCode = -99;
                if (string.IsNullOrEmpty(facebookUserName))
                    return Ok(new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) });
                var fbPassword = FacebookHelper.GetFacebookPassword(facebookUserName);
                var passMd5 = AccountUtils.EncryptPasswordMd5(fbPassword);
                responseCode = await Task.Run(async () =>
                {
                    int accountId = 0;
                    var res = StoreUsersSqlInstance.Inst.Register(REGISTER_TYPE.FACEBOOK_TYPE, facebookUserName, "", passMd5, clientInfo.MerchantId, clientInfo.TrueClientIp, clientInfo.DeviceId, (int)clientInfo.OsType,
                        requestLogin.PhoneNumber, requestLogin.Email, out accountId);

                    if (responseCode == (int)EStatusCode.SUCCESS)
                    {
                        await _logger.LogInfo("Account-LoginGoogle{}", facebookUserName + " " + res, res.ToString()).ConfigureAwait(false);
                        MailInstance.Inst.SendMailRegis(accountId);
                        if (AccountUtils.IsValidEmail(requestLogin.Email))
                            await SendMailAsync(requestLogin.Email, accountId).ConfigureAwait(false);
                    }
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
                    return Ok(new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) });
                }
            }
            catch (Exception ex)
            {
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR), DataResponse = ex.ToString() });
            }
        }


        [HttpPost]
        [Route("LoginFacebook")]
        public async Task<IActionResult> LoginFacebook(RequestAuthenSocial requestLogin)
        {
            if (!AccountUtils.IsLoginRequestTrue(requestLogin)) {
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.DATA_INVAILD, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.DATA_INVAILD) });
            }
            if (!string.IsNullOrEmpty(requestLogin.Email))
                if (!AccountUtils.IsValidEmail(requestLogin.Email))
                    return Ok(new ResponseApiModel<string>() { Status = EStatusCode.EMAIL_INVAILD, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.EMAIL_INVAILD) });
            if (!string.IsNullOrEmpty(requestLogin.PhoneNumber))
                if (!AccountUtils.IsPhoneNumber(requestLogin.PhoneNumber))
                    return Ok(new ResponseApiModel<string>() { Status = EStatusCode.PHONE_INVAILD, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.PHONE_INVAILD) });

            try
            {
                var clientInfo = new ClientRequestInfo(Request);

                var facebookUserName = await FacebookHelper.GetFacebookUserNameAsync(requestLogin.Token);
                await _logger.LogError("Account-Register{}", facebookUserName).ConfigureAwait(false);
                var responseCode = -99;
                if (string.IsNullOrEmpty(facebookUserName))
                    return Ok(new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) });
                var fbPassword = FacebookHelper.GetFacebookPassword(facebookUserName);
                var passMd5 = AccountUtils.EncryptPasswordMd5(fbPassword);
                responseCode = await Task.Run(async () =>
                {
                    int accountId = 0;
                    var res = StoreUsersSqlInstance.Inst.Register(REGISTER_TYPE.FACEBOOK_TYPE, facebookUserName, "", passMd5, clientInfo.MerchantId, clientInfo.TrueClientIp, clientInfo.DeviceId, (int)clientInfo.OsType,
                        requestLogin.PhoneNumber, requestLogin.Email, out accountId);

                    if (responseCode == (int)EStatusCode.SUCCESS)
                    {
                        await _logger.LogInfo("Account-RegisterFB{}", facebookUserName + " " + res, res.ToString()).ConfigureAwait(false);
                        MailInstance.Inst.SendMailRegis(accountId);
                        if (AccountUtils.IsValidEmail(requestLogin.Email))
                            await SendMailAsync(requestLogin.Email, accountId).ConfigureAwait(false);
                    }
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
                    return Ok(new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) });
                }
            }
            catch (Exception ex)
            {
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR), DataResponse= ex.ToString() });
            }
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Regis(RequestRegisterModel requestRegis)
        {
            if (!AccountUtils.IsRegisterRequestTrue(requestRegis)) {
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.DATA_INVAILD, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.DATA_INVAILD) });
            }
            if (!string.IsNullOrEmpty(requestRegis.Email))
                if (!AccountUtils.IsValidEmail(requestRegis.Email))
                    return Ok(new ResponseApiModel<string>() { Status = EStatusCode.EMAIL_INVAILD, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.EMAIL_INVAILD) });
            if (!string.IsNullOrEmpty(requestRegis.PhoneNumber))
                if (!AccountUtils.IsPhoneNumber(requestRegis.PhoneNumber))
                    return Ok(new ResponseApiModel<string>() { Status = EStatusCode.PHONE_INVAILD, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.PHONE_INVAILD) });

            var clientInfo = new ClientRequestInfo(Request);
            var responseCode = -99;
            try
            {
                responseCode = await Task.Run(async () =>
                {
                    int accountId = 0;
                    var res = StoreUsersSqlInstance.Inst.Register(REGISTER_TYPE.NORMAL_TYPE, requestRegis.AccountName, "", AccountUtils.EncryptPasswordMd5(requestRegis.Password), clientInfo.MerchantId, clientInfo.TrueClientIp, clientInfo.DeviceId, (int)clientInfo.OsType,
                        requestRegis.PhoneNumber, requestRegis.Email, out accountId);

                    if (responseCode == (int)EStatusCode.SUCCESS)
                    {
                        await _logger.LogInfo("Account-Register{}", requestRegis.AccountName + " " + res, res.ToString()).ConfigureAwait(false);
                        MailInstance.Inst.SendMailRegis(accountId);
                        if (AccountUtils.IsValidEmail(requestRegis.Email))
                            await SendMailAsync(requestRegis.Email, accountId).ConfigureAwait(false);
                    }
                    return res;

                });
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
                    return Ok(new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) });
                }
            }
            catch (Exception ex)
            {
                await _logger.LogError("Account-Register{}", ex.ToString()).ConfigureAwait(false);
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.SUCCESS, Messenger = UtilsSystem.Utils.UltilsHelper.GetMessageByErrorCode(EStatusCode.SUCCESS) });
            }
        }

        private async Task<ResponseApiModel<TokenInfo>> LoginAsync(RequestAuthenModel requestLogin, ClientRequestInfo clientInfo)
        {
            ResponseApiModel<TokenInfo> model = new ResponseApiModel<TokenInfo>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            try
            {
                var res = EStatusCode.SUCCESS;
                res = await Task.Run(async () =>
                {
                    var accountId = StoreUsersSqlInstance.Inst.DoLogin(requestLogin.AccountName, AccountUtils.EncryptPasswordMd5(requestLogin.Password), clientInfo.MerchantId, clientInfo.TrueClientIp, (int)clientInfo.OsType, ref res);

                    await _logger.LogInfo("Account-LoginAsync{}", requestLogin.AccountName + " - accountId:" + accountId+ " - res:" + res, res.ToString()).ConfigureAwait(false);
                    if (accountId >= 0)
                    {
                        //create refresh token -> save db
                        var refreshToken = _tokenManager.GenerateRefreshToken();
                        var responseToken = StoreUsersSqlInstance.Inst.AddToken(accountId, refreshToken);
                        if (responseToken >= 0)
                        {
                            var accessToken = await _tokenManager.GenerateAccessTokenAsync(accountId, clientInfo);
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
        public async Task<IActionResult> RefreshToken(TokenInfo data)
        {
            try
            {
                long accountId = 0;
                //check token
                var response = StoreUsersSqlInstance.Inst.CheckRefreshToken(data.Refresh_token, ref accountId);
                await _logger.LogInfo("Account-RefreshToken{}", data.Refresh_token + " - " + accountId + " - " + response, response.ToString()).ConfigureAwait(false);
                if (response >= 0)
                {
                    var clientInfo = new ClientRequestInfo(Request);
                    var accessToken = await _tokenManager.GenerateAccessTokenAsync(accountId, clientInfo);
                    var model = new TokenInfo() { AccountId = accountId, Access_token = accessToken, Refresh_token = data.Refresh_token };
                    return Ok(new ResponseApiModel<TokenInfo>() { Status = EStatusCode.SUCCESS, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SUCCESS), DataResponse = model });
                }
                else if (response == EStatusCode.TOKEN_INVALID)
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
            return Ok(new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) });
        }

        [HttpGet]
        [Route("GetAccountInfo")]
        public async Task<IActionResult> GetAccountInfo()
        {
            int responseStatus = -99;
            AccountModelDb response = null;
            try
            {
                long accountId = await _tokenManager.GetAccountIdByAccessTokenAsync(Request);
                if (accountId <= 0)
                    return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });
                response = StoreUsersSqlInstance.Inst.GetAccountInfo(accountId, ref responseStatus);
            }
            catch (Exception ex)
            {
                await _logger.LogError("Account-GetAccountInfo{}", ex.ToString()).ConfigureAwait(false);
            }

            return Ok(new ResponseApiModel<AccountModel>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = new AccountModel(response) });
        }

        [HttpPost]
        [Route("UpdateEmail")]
        public async Task<IActionResult> UpdateEmail(string Email)
        {
            string message = "";
            int responseStatus = -99;
            AccountModelDb response = null;
            try
            {
                long accountId = await _tokenManager.GetAccountIdByAccessTokenAsync(Request);
                if (accountId <= 0)
                    return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });
                response = StoreUsersSqlInstance.Inst.UpdateEmail(accountId, Email, ref responseStatus);
                await _logger.LogInfo("Account-UpdateEmail{}", Email + " - accountId:" + accountId + " - " + response, response.ToString()).ConfigureAwait(false);
                if (responseStatus == 0)
                {
                    await SendMailAsync(Email, accountId);
                    message = UltilsHelper.GetMessageByErrorCode(EStatusCode.EMAIL_SEND);
                }
                else {
                    message = UltilsHelper.GetMessageByErrorCode(responseStatus);
                }
            }
            catch (Exception ex)
            {
                await _logger.LogError("Account-UpdateEmail{}", ex.ToString()).ConfigureAwait(false);
            }

            return Ok(new ResponseApiModel<AccountModel>() { Status = responseStatus, Messenger = message, DataResponse = new AccountModel(response) });
        }
        [HttpPost]
        [Route("UpdateInfo")]
        public async Task<IActionResult> UpdateInfo(RequestUpdateInfoModel model)
        {
            int responseStatus = -99;
            AccountModelDb response = null;
            try
            {
                long accountId = await _tokenManager.GetAccountIdByAccessTokenAsync(Request);
                if (accountId <= 0)
                    return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });
                response = StoreUsersSqlInstance.Inst.UpdateInfo(accountId, model.Nickname, model.AvatarId, model.PhoneNumber, model.BirthDay, model.Adress, ref responseStatus);
                await _logger.LogInfo("Account-UpdateInfo{}", JsonConvert.SerializeObject(model) + " - accountId:" + accountId + " - " + response, response.ToString()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _logger.LogError("Account-UpdateInfo{}", ex.ToString()).ConfigureAwait(false);
            }

            return Ok(new ResponseApiModel<AccountModel>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = new AccountModel(response) });
        }

        [HttpGet]
        [Route("GetBookBuy")]
        //[ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetBookBuy(int page = 1, int row = 100)
        {
            if (row > 100)
                row = 100;
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                long accountId = await _tokenManager.GetAccountIdByAccessTokenAsync(Request);
                if (accountId <= 0)
                    return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });
                string keyRedis = "CacheBookBuy:" + accountId+":"+page+"-"+row;
                string jsonListSimpleBook = await RedisGatewayCacheManager.Inst.GetDataFromCacheAsync(keyRedis);
                if (string.IsNullOrEmpty(jsonListSimpleBook))
                {
                    var listBook = StoreBookSqlInstance.Inst.GetBookBuyAccount(accountId, page, row, out responseStatus);
                    if (responseStatus == EStatusCode.SUCCESS)
                    {
                        jsonListSimpleBook = JsonConvert.SerializeObject(listBook);
                        await RedisGatewayCacheManager.Inst.SaveDataAsync(keyRedis, jsonListSimpleBook, 10);
                    }
                }
                else
                {
                    responseStatus = EStatusCode.SUCCESS;
                }
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = jsonListSimpleBook };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Account-GetBookBuy{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("GetCountBookBuy")]
        public async Task<IActionResult> GetCountBookBuy()
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                long accountId = await _tokenManager.GetAccountIdByAccessTokenAsync(Request);
                if (accountId <= 0)
                    return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });
                string keyRedis = "CacheCountBookBuy:" + accountId;
                string jsonListSimpleBook = await RedisGatewayCacheManager.Inst.GetDataFromCacheAsync(keyRedis);
                if (string.IsNullOrEmpty(jsonListSimpleBook))
                {
                    long countBuy = StoreBookSqlInstance.Inst.GetCountBuyAccount(accountId, out responseStatus);
                    if (responseStatus == EStatusCode.SUCCESS)
                    {
                        jsonListSimpleBook = JsonConvert.SerializeObject(countBuy);
                        await RedisGatewayCacheManager.Inst.SaveDataAsync(keyRedis, jsonListSimpleBook, 10);
                    }
                }
                else
                {
                    responseStatus = EStatusCode.SUCCESS;
                }
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = jsonListSimpleBook };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Account-GetCountBookBuy{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("BuyBook")]
        public async Task<IActionResult> BuyBook(string barcode)
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            long curentVip = 0;
            long curentPoint = 0;
            bool isNextLevel = false;
            long point = 0;
            string vipName = "";
            try
            {
                long accountId = await _tokenManager.GetAccountIdByAccessTokenAsync(Request);
                if (accountId <= 0)
                    return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });
                var model = StoreBookSqlInstance.Inst.AccountBuyBarcode(accountId, barcode, out responseStatus, out point);
                if (responseStatus == EStatusCode.SUCCESS)
                {
                    string keyRedis = "CacheBookBuy:" + accountId;
                    await RedisGatewayCacheManager.Inst.DeleteArrayKeyAsync(keyRedis).ConfigureAwait(false);
                    keyRedis = "CacheCountBookBuy:" + accountId;
                    await RedisGatewayCacheManager.Inst.DeleteDataFromCacheAsync(keyRedis).ConfigureAwait(false);
                    //update vip value
                    string rewardLevelUp = "";
                    var responseStatusMail = StoreMemberSqlInstance.Inst.IncPointAccountByBook(accountId, (long)(point/1000), out curentPoint, out curentVip, out isNextLevel, out vipName, out rewardLevelUp);
                    //if (isNextLevel) {
                    //    string mailHeader = "Chúc mừng bạn lên hạng!";
                    //    string mailContent = "Tặng bạn phần quà là vourcher "+ rewardLevelUp;
                    //    var mail = StoreMailSqlInstance.Inst.SendMail(accountId, "admin", mailHeader, mailContent, out responseStatusMail);
                    //}
                    await _logger.LogBuyBook("Account-BuyBook{}", accountId, barcode, point).ConfigureAwait(false);

                }
                ResponseBuyBook responseBuyBook = new ResponseBuyBook() { modelBook = model, modelMember = new ResponseMemberVip() { CurrentPoint = curentPoint, CurrentVip = curentVip, VipName = vipName }, pointBook = (long)(point / 1000), levelUp = isNextLevel };
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = JsonConvert.SerializeObject(responseBuyBook) };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Account-BuyBook{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("GetLikeBook")]
        public async Task<IActionResult> GetLikeBook(int page, int row)
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            long accountId = await _tokenManager.GetAccountIdByAccessTokenAsync(Request);
            if (accountId <= 0)
                return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });
            try
            {
                string keyRedis = "CacheLikeBook:" + accountId+":"+page+"-"+row;
                string jsonListSimpleBook = await RedisGatewayCacheManager.Inst.GetDataFromCacheAsync(keyRedis);
                if (string.IsNullOrEmpty(jsonListSimpleBook))
                {
                    var downloadInfoModel = StoreBookSqlInstance.Inst.GetLikeBook(accountId, page, row, out responseStatus);
                    if (responseStatus == EStatusCode.SUCCESS)
                    {
                        jsonListSimpleBook = JsonConvert.SerializeObject(downloadInfoModel);
                        await RedisGatewayCacheManager.Inst.SaveDataAsync(keyRedis, jsonListSimpleBook, 10);
                    }
                }
                else
                {
                    responseStatus = EStatusCode.SUCCESS;
                }
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = jsonListSimpleBook };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Account-GetLikeBook{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("GetCountLikeBook")]
        public async Task<IActionResult> GetCountLikeBook()
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            long accountId = await _tokenManager.GetAccountIdByAccessTokenAsync(Request);
            if (accountId <= 0)
                return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });
            try
            {
                string keyRedis = "CacheCountLikeBook:" + accountId;
                string jsonListSimpleBook = await RedisGatewayCacheManager.Inst.GetDataFromCacheAsync(keyRedis);
                if (string.IsNullOrEmpty(jsonListSimpleBook))
                {
                    long countBuy = StoreBookSqlInstance.Inst.GetCountBookLike(accountId, out responseStatus);
                    if (responseStatus == EStatusCode.SUCCESS)
                    {
                        jsonListSimpleBook = JsonConvert.SerializeObject(countBuy);
                        await RedisGatewayCacheManager.Inst.SaveDataAsync(keyRedis, jsonListSimpleBook, 10);
                    }
                }
                else
                {
                    responseStatus = EStatusCode.SUCCESS;
                }
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = jsonListSimpleBook };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Account-GetCountLikeBook{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("GetMemberVip")]
        public async Task<IActionResult> GetMemberVip()
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            long accountId = await _tokenManager.GetAccountIdByAccessTokenAsync(Request);
            if (accountId <= 0)
                return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });
            long curentPoint = 0;
            long curentVip = 0;
            string vipName = "";
            try
            {
                string keyRedis = "GetMemberVip:" + accountId;
                string jsonListSimpleBook = await RedisGatewayCacheManager.Inst.GetDataFromCacheAsync(keyRedis);
                if (string.IsNullOrEmpty(jsonListSimpleBook))
                {
                    StoreMemberSqlInstance.Inst.GetCurrentPointAccount(accountId, out curentPoint, out curentVip, out vipName, out responseStatus);
                    if (responseStatus == EStatusCode.SUCCESS)
                    {
                        ResponseMemberVip model = new ResponseMemberVip() { CurrentPoint = curentPoint, CurrentVip = curentVip, VipName = vipName };
                        jsonListSimpleBook = JsonConvert.SerializeObject(model);
                        await RedisGatewayCacheManager.Inst.SaveDataAsync(keyRedis, jsonListSimpleBook, 1);
                    }
                }
                else
                {
                    responseStatus = EStatusCode.SUCCESS;
                }
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = jsonListSimpleBook };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Account-GetCountLikeBook{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }


        [HttpGet]
        [Route("GetVourcherAccount")]
        public async Task<IActionResult> GetVourcherAccount()
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            long accountId = await _tokenManager.GetAccountIdByAccessTokenAsync(Request);
            if (accountId <= 0)
                return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });
            try
            {
                string keyRedis = "GetVourcherAccount:" + accountId;
                string jsonstring = await RedisGatewayCacheManager.Inst.GetDataFromCacheAsync(keyRedis);
                if (string.IsNullOrEmpty(jsonstring))
                {
                    var listModel = StoreUsersSqlInstance.Inst.GetVourcherAccount(accountId, out responseStatus);
                    if (responseStatus == EStatusCode.SUCCESS)
                    {
                        jsonstring = JsonConvert.SerializeObject(listModel);
                        await RedisGatewayCacheManager.Inst.SaveDataAsync(keyRedis, jsonstring, 1);
                    }
                }
                else
                {
                    responseStatus = EStatusCode.SUCCESS;
                }
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = jsonstring };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Account-GetCountLikeBook{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }
        //[HttpPost]
        //[Route("TestSendMail")]
        //[ResponseCache(Duration = 5)]
        //public async Task<IActionResult> TestSendMail(string toAdress)
        //{
        //    int responseStatus = 0;
        //    try
        //    {
        //        var message = new Message(new string[] { toAdress }, "Test email", "This is the content from our email. Tesst validate link: https://dev-launcherlogic.vplay.vn/test/v1/NotifyServices/GetNotifyAdmin ");
        //        await _emailSender.SendEmailAsync(message).ConfigureAwait(false);
        //    }
        //    catch (Exception ex)
        //    {
        //        await _logger.LogError("Account-UpdateInfo{}", JsonConvert.SerializeObject(CONFIG.EmailConfig) + " " + ex.ToString()).ConfigureAwait(false);
        //    }

        //    return Ok(new ResponseApiLauncher<AccountModel>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus) });
        //}

        private async Task SendMailAsync(string toAdress, long AccountId){
            return;
            string key = HttpUtility.UrlEncode(await _tokenManager.GenerateKeyTokenValidateAsync(AccountId, toAdress));
            string url = CONFIG.BaseLink + CONFIG.FunctionValidateEmail+ key;

            var message = new Message(new string[] { toAdress }, "Validate email", "This is the content from our email, token lifetime 10 min. Click with validate link: "+ url);
            await _emailSender.SendEmailAsync(message).ConfigureAwait(false);
        }
       
    }
}
