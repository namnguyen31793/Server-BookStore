using BookStoreCMS.Instance;
using BookStoreCMS.Utils;
using DAO.DAOImp;
using LoggerService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RedisSystem;
using ShareData.API;
using ShareData.DataEnum;
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

namespace BookStoreCMS.Controllers
{
    [ApiVersion("1.0")]
    [Route("v1/AccountCms")]
    [ApiController]
    public class AccountCmsController : ControllerBase
    {

        private ILoggerManager _logger;
        private IEmailSender _emailSender;
        public AccountCmsController(ILoggerManager logger, IEmailSender emailSender)
        {
            _logger = logger;
            _emailSender = emailSender;
        }
        private string token = string.Empty;

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(RequestAuthenModel requestLogin)
        {
            var response = new ResponseApiModel<TokenInfoCms>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
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
        /*
         Chỉ có quyền login k có quyền tạo mới
         */
        [HttpPost]
        [Route("LoginFacebook")]
        public async Task<IActionResult> LoginFacebook(RequestAuthenSocial requestLogin)
        {
            if (!AccountUtils.IsLoginRequestTrue(requestLogin)) {
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.DATA_INVAILD, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.DATA_INVAILD) });
            }
            var clientInfo = new ClientRequestInfo(Request);

            var facebookUserName = await FacebookHelper.GetFacebookUserNameAsync(requestLogin.Token);
            if (string.IsNullOrEmpty(facebookUserName))
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) });
            var fbPassword = FacebookHelper.GetFacebookPassword(facebookUserName);
            var passMd5 = AccountUtils.EncryptPasswordMd5(fbPassword);

            RequestAuthenModel request = new RequestAuthenModel()
            {
                AccountName = facebookUserName,
                Password = fbPassword,
            };
            var response = await LoginAsync(request, clientInfo);
            return Ok(response);
        }

        private async Task<ResponseApiModel<TokenInfoCms>> LoginAsync(RequestAuthenModel requestLogin, ClientRequestInfo clientInfo)
        {
            ResponseApiModel<TokenInfoCms> model = new ResponseApiModel<TokenInfoCms>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            try
            {
                var res = EStatusCode.SUCCESS;
                res = await Task.Run(() =>
                {
                    int role = 0;
                    var accountId = StoreUsersSqlInstance.Inst.DoLoginCms(requestLogin.AccountName, AccountUtils.EncryptPasswordMd5(requestLogin.Password), clientInfo.MerchantId, clientInfo.TrueClientIp, (int)clientInfo.OsType, ref role, ref res);
                    if (accountId >= 0)
                    {
                        if (role != ERole.GM || role != ERole.Administrator)
                            return res = EStatusCode.ACCOUNT_NOT_ENOUGH_ROLE;
                        //create refresh token -> save db
                        var refreshToken = TokenCMSManager.GenerateRefreshToken();
                        var responseToken = StoreUsersSqlInstance.Inst.AddToken(accountId, refreshToken);
                        if (responseToken >= 0)
                        {
                            var accessToken = TokenCMSManager.GenerateAccessToken(accountId, role, clientInfo);
                            model.DataResponse = new TokenInfoCms() { Role = role, AccountId = accountId, Access_token = accessToken, Refresh_token = refreshToken };
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
                long accountId = 0;
                int role = 0;
                //check token
                var response = StoreUsersSqlInstance.Inst.CheckRefreshTokenCms(data.Refresh_token, ref role, ref accountId);
                if (response >= 0)
                {
                    var clientInfo = new ClientRequestInfo(Request);
                    var accessToken = TokenCMSManager.GenerateAccessToken(accountId, role, clientInfo);
                    var model = new TokenInfoCms() { Role = role, AccountId = accountId, Access_token = accessToken, Refresh_token = data.Refresh_token };
                    return Ok(new ResponseApiModel<TokenInfoCms>() { Status = EStatusCode.SUCCESS, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SUCCESS), DataResponse = model });
                }
                else if (response == EStatusCode.TOKEN_INVALID)
                {
                    return Ok(new ResponseApiModel<TokenInfoCms>() { Status = EStatusCode.TOKEN_INVALID, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.TOKEN_INVALID) });
                }
                else if (response == EStatusCode.TOKEN_EXPIRES)
                {
                    return Ok(new ResponseApiModel<TokenInfoCms>() { Status = EStatusCode.TOKEN_EXPIRES, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.TOKEN_EXPIRES) });
                }
            }
            catch (Exception ex)
            {
                await _logger.LogError("Account-RefreshToken{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) });
        }

        [HttpGet]
        [Route("GetAccountInfoById")]
        [ResponseCache(Duration = 5)]
        public async Task<IActionResult> GetAccountInfo(long accountId)
        {
            int responseStatus = -99;
            AccountModelDb response = null;
            try
            {
                int role = 0;
                int checkRole = TokenCMSManager.CheckRoleAction(ERole.Administrator, Request);
                if(checkRole < 0)
                    return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });
                response = StoreUsersSqlInstance.Inst.GetAccountInfo(accountId, ref responseStatus);
            }
            catch (Exception ex)
            {
                await _logger.LogError("Account-GetAccountInfo{}", ex.ToString()).ConfigureAwait(false);
            }

            return Ok(new ResponseApiModel<AccountModel>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = new AccountModel(response) });
        }
    }
}
