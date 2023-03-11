using BookStore.Interfaces;
using BookStore.Utils;
using DAO.DAOImp;
using LoggerService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RedisSystem;
using ShareData.API;
using ShareData.ErrorCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UtilsSystem.Utils;

namespace BookStore.Controllers
{
    [ApiVersion("1.0")]
    [Route("v1/Configs")]
    [ApiController]
    public class ConfigsController : Controller
    {

        private ILoggerManager _logger;
        private ITokenManager _tokenManager;
        public ConfigsController(ILoggerManager logger, ITokenManager token)
        {
            _logger = logger;
            _tokenManager = token;
        }

        [HttpGet]
        [Route("GetMerberConfig")]
        [ResponseCache (Duration = 60)]
        public async Task<IActionResult> GetMerberConfig()
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                string keyRedis = "GetMerberConfig";
                string DataListMail = await RedisGatewayCacheManager.Inst.GetDataFromCacheAsync(keyRedis);
                if (string.IsNullOrEmpty(DataListMail))
                {
                    var data = StoreMemberSqlInstance.Inst.GetVipInfoModel(out responseStatus);
                    if (responseStatus == EStatusCode.SUCCESS)
                    {
                        DataListMail = JsonConvert.SerializeObject(data);
                        await RedisGatewayCacheManager.Inst.SaveDataAsync(keyRedis, DataListMail, 10);
                    }
                }
                else
                {
                    responseStatus = EStatusCode.SUCCESS;
                }
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = DataListMail };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Configs-GetMerberConfig{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }
        [HttpGet]
        [Route("GetCcu")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> GetCcu()
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                string keyRedis = "Token";
                long ccu = await RedisGatewayCacheManager.Inst.CountItemByString(keyRedis);
                responseStatus = 0;
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = ccu.ToString() };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Configs-GetCcu{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("UploadAvata")]
        [RequestSizeLimit(5*1024*1024)]
        public async Task<IActionResult> UploadAvata(IFormFile file)
        {
            long accountId = await _tokenManager.GetAccountIdByAccessTokenAsync(Request);
            if (accountId <= 0)
                return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });

            return Ok();
        }
    }
}
