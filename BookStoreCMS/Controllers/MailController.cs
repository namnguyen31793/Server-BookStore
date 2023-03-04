using BookStoreCMS.Instance;
using BookStoreCMS.Interfaces;
using BookStoreCMS.Utils;
using DAO.DAOImp;
using LoggerService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RedisSystem;
using ShareData.API;
using ShareData.DataEnum;
using ShareData.DB.Mail;
using ShareData.ErrorCode;
using ShareData.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UtilsSystem.Utils;

namespace BookStoreCMS.Controllers
{
    [ApiVersion("1.0")]
    [Route("v1/Email")]
    [ApiController]
    public class MailController : ControllerBase
    {

        private ILoggerManager _logger;
        private IEmailSender _emailSender;
        private ITokenManager _tokenManager;
        public MailController(ILoggerManager logger, IEmailSender emailSender, ITokenManager tokenManager)
        {
            _logger = logger;
            _emailSender = emailSender;
            _tokenManager = tokenManager;
        }
        [HttpGet]
        [ResponseCache(Duration = 5)]
        [Route("GetMailUser")]
        public async Task<IActionResult> GetMailUser(long AccountId, int page = 1, int row = 100)
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            try
            {
                int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
                if (checkRole < 0)
                    return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });
                var model = StoreMailSqlInstance.Inst.GetAllMailListByAccountId(AccountId, page, row);

                response = new ResponseApiModel<string>() { Status = EStatusCode.SUCCESS, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SUCCESS), DataResponse = JsonConvert.SerializeObject(model) };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Account-GetMailUser{}", ex.ToString()).ConfigureAwait(false);
            }

            return Ok(response);
        }

        [HttpPost]
        [ResponseCache(Duration = 5)]
        [Route("SendMailUser")]
        public async Task<IActionResult> SendMailUser(RequestSendMailModel data)
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            try
            {
                int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
                if (checkRole < 0)
                    return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });
                int res = EStatusCode.DATABASE_ERROR;
                var model = StoreMailSqlInstance.Inst.SendMail(data.Accountid, data.SenderNickname, data.MailHeader, data.MailContent, out res, data.Money, data.RewardBonusDescription);

                response = new ResponseApiModel<string>() { Status = res, Messenger = UltilsHelper.GetMessageByErrorCode(res), DataResponse = JsonConvert.SerializeObject(model) };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Account-SendMailUser{}", ex.ToString()).ConfigureAwait(false);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("RestoreMail")]
        public async Task<IActionResult> RestoreMail(long MailId)
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };

            long accountId = await _tokenManager.GetAccountIdByAccessTokenAsync(Request);
            if (accountId <= 0)
                return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });
            try
            {
                int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
                if (checkRole < 0)
                    return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });
                int res = EStatusCode.DATABASE_ERROR;
                var model = StoreMailSqlInstance.Inst.RestoreMail(MailId, out res);

                response = new ResponseApiModel<string>() { Status = res, Messenger = UltilsHelper.GetMessageByErrorCode(res), DataResponse = JsonConvert.SerializeObject(model) };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Account-Login{}", ex.ToString()).ConfigureAwait(false);
            }

            return Ok(response);
        }
    }
}
