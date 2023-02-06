using BookStore.Instance;
using BookStore.Utils;
using DAO.DAOImp;
using LoggerService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RedisSystem;
using ShareData.API;
using ShareData.DB.Mail;
using ShareData.ErrorCode;
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
        public MailController(ILoggerManager logger, IEmailSender emailSender)
        {
            _logger = logger;
            _emailSender = emailSender;
        }

        [HttpGet]
        [ResponseCache(Duration = 5)]
        [Route("GetUserMail")]
        public async Task<IActionResult> GetUserMail()
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            long accountId = TokenManager.GetAccountIdByAccessToken(Request);
            if (accountId <= 0)
                return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });
            try
            {
                string valueString = GetListMailById(accountId);
                response = new ResponseApiModel<string>() { Status = EStatusCode.SUCCESS, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SUCCESS), DataResponse = valueString };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Account-GetUserMail{}", ex.ToString()).ConfigureAwait(false);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("ReadMail")]
        public async Task<IActionResult> ReadMail(long MailId)
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };

            long accountId = TokenManager.GetAccountIdByAccessToken(Request);
            if (accountId <= 0)
                return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });
            try
            {
                string valueString = GetListMailById(accountId);
                if(string.IsNullOrEmpty(valueString))
                    return Ok(new ResponseApiModel<string>() { Status = EStatusCode.MAIL_NOT_EXIST, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.MAIL_NOT_EXIST) });
                List<MailObject> listMail = JsonConvert.DeserializeObject <List<MailObject>>(valueString);
                int responseStatus = EStatusCode.DATABASE_ERROR;
                if (listMail.Exists(x => x.MailId == MailId))
                    StoreMailSqlInstance.Inst.UpdateReadedMail(MailId, out responseStatus);
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus) };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Account-Login{}", ex.ToString()).ConfigureAwait(false);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("DeleteMail")]
        public async Task<IActionResult> DeleteMail(long MailId)
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };

            long accountId = TokenManager.GetAccountIdByAccessToken(Request);
            if (accountId <= 0)
                return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });
            try
            {
                string valueString = GetListMailById(accountId);
                if (string.IsNullOrEmpty(valueString))
                    return Ok(new ResponseApiModel<string>() { Status = EStatusCode.MAIL_NOT_EXIST, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.MAIL_NOT_EXIST) });
                List<MailObject> listMail = JsonConvert.DeserializeObject<List<MailObject>>(valueString);
                int responseStatus = EStatusCode.DATABASE_ERROR;
                string jsonListMail = "";
                if (listMail.Exists(x => x.MailId == MailId))
                {
                    listMail = StoreMailSqlInstance.Inst.DeleteMail(accountId, MailId, out responseStatus);
                    if (responseStatus == EStatusCode.SUCCESS){
                        jsonListMail = JsonConvert.SerializeObject(listMail);
                        string keyRedis = "CacheMail:" + accountId;
                        RedisGatewayManager<string>.Inst.SaveData(keyRedis, jsonListMail, 300);
                    }
                }
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = jsonListMail };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Account-Login{}", ex.ToString()).ConfigureAwait(false);
            }

            return Ok(response);
        }

        private string GetListMailById(long accountId) {
            string keyRedis = "CacheMail:" + accountId;
            string valueString = RedisGatewayManager<string>.Inst.GetDataFromCache(keyRedis);
            if (string.IsNullOrEmpty(valueString))
            {
                var model = StoreMailSqlInstance.Inst.GetMailListByAccountId(accountId);
                valueString = JsonConvert.SerializeObject(model);
                RedisGatewayManager<string>.Inst.SaveData(keyRedis, valueString, 300);
            }
            return valueString;
        }
    }
}
