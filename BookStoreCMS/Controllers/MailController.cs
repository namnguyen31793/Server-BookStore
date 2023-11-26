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

                int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
                if (checkRole < 0)
                    return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });
                var model = StoreMailSqlInstance.Inst.GetAllMailListByAccountId(AccountId, page, row);

                response = new ResponseApiModel<string>() { Status = EStatusCode.SUCCESS, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SUCCESS), DataResponse = JsonConvert.SerializeObject(model) };


            return Ok(response);
        }

        [HttpPost]
        [Route("SendMailUser")]
        public async Task<IActionResult> SendMailUser(RequestSendMailModel data)
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };

            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });
            int res = EStatusCode.DATABASE_ERROR;
            var model = StoreMailSqlInstance.Inst.SendMail(data.Accountid, data.SenderNickname, data.MailHeader, data.MailContent, out res, data.Money, data.RewardBonusDescription);

            response = new ResponseApiModel<string>() { Status = res, Messenger = UltilsHelper.GetMessageByErrorCode(res), DataResponse = JsonConvert.SerializeObject(model) };


            return Ok(response);
        }

        [HttpPost]
        [Route("SendMailAllUser")]
        public async Task<IActionResult> SendMailAllUser(RequestSendMailModel data)
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };

            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });
            int res = EStatusCode.SUCCESS;
            StoreMailSqlInstance.Inst.SendAllMail(data.SenderNickname, data.MailHeader, data.MailContent, out res, data.Money, data.RewardBonusDescription);

            response = new ResponseApiModel<string>() { Status = res, Messenger = UltilsHelper.GetMessageByErrorCode(res) };

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
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });
            int res = EStatusCode.DATABASE_ERROR;
            var model = StoreMailSqlInstance.Inst.RestoreMail(MailId, out res);

            response = new ResponseApiModel<string>() { Status = res, Messenger = UltilsHelper.GetMessageByErrorCode(res), DataResponse = JsonConvert.SerializeObject(model) };

            return Ok(response);
        }

        #region EMAIL
        [HttpGet]
        [ResponseCache(Duration = 5)]
        [Route("GetListEmailAdmin")]
        public async Task<IActionResult> GetListEmailAdmin()
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            try
            {
                int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
                if (checkRole < 0)
                    return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });
                var model = StoreMailSqlInstance.Inst.GetListEmailAdmin();

                response = new ResponseApiModel<string>() { Status = EStatusCode.SUCCESS, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SUCCESS), DataResponse = JsonConvert.SerializeObject(model) };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Account-GetMailUser{}", ex.ToString()).ConfigureAwait(false);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("AddEmailAdmin")]
        public async Task<IActionResult> AddEmailAdmin(RequestEmaiModel request)
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = 0;

            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });
            var model = StoreMailSqlInstance.Inst.AddEmailAdmin(request.Username, request.Password, request.Status, out responseStatus);

            response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = JsonConvert.SerializeObject(model) };

            return Ok(response);
        }

        [HttpPost]
        [Route("UpdateEmailAdmin")]
        public async Task<IActionResult> UpdateEmailAdmin(RequestEmaiModel request)
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = 0;

            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });
            var model = StoreMailSqlInstance.Inst.UpdateEmailAdmin(request.MailId, request.Username, request.Password, request.Status, out responseStatus);

            response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = JsonConvert.SerializeObject(model) };

            return Ok(response);
        }
        #endregion
        
        #region NOTIFY MAIL
        [HttpPost]
        [Route("AddNotifyMailCms")]
        public async Task<IActionResult> AddNotifyMailCms(RequestSendNotifyMailModel data)
        {
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });
            int res = EStatusCode.SUCCESS;
            var mail = StoreMailSqlInstance.Inst.AddNotifyMailCms(data.SenderNickname, data.MailHeader, data.MailContent, out res, data.RewardDescription);

            var response = new ResponseApiModel<string>() { Status = res, Messenger = UltilsHelper.GetMessageByErrorCode(res), DataResponse = JsonConvert.SerializeObject(mail) };

            return Ok(response);
        }

        [HttpPost]
        [Route("DeleteNotifyMailCms")]
        public async Task<IActionResult> DeleteNotifyMailCms(long MailId)
        {
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });

            int responseStatus = EStatusCode.DATABASE_ERROR;
            var listMail = StoreMailSqlInstance.Inst.DeleteNotifyMailCms(MailId, out responseStatus);
            var response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = JsonConvert.SerializeObject(listMail) };

            return Ok(response);
        }
        [HttpGet]
        [Route("GetNotifyMailCms")]
        public async Task<IActionResult> GetNotifyMailCms()
        {
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });

            int responseStatus = EStatusCode.DATABASE_ERROR;
            var listMail = StoreMailSqlInstance.Inst.GetNotifyMailCms(out responseStatus);
            var response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = JsonConvert.SerializeObject(listMail) };

            return Ok(response);
        }
        #endregion
    }
}
