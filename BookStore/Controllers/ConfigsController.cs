﻿using BookStore.Utils;
using DAO.DAOImp;
using LoggerService;
using Marvin.Cache.Headers;
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
        public ConfigsController(ILoggerManager logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("GetMerberConfig")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 10)]
        [HttpCacheValidation(MustRevalidate = false)]
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
    }
}