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
    [Route("v1/Books")]
    [ApiController]
    public class BooksController : ControllerBase
    {

        private ILoggerManager _logger;
        public BooksController(ILoggerManager logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("{barcode}/GetListComment")]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetListComment(string barcode)
        {
            if(string.IsNullOrEmpty(barcode))
            return Ok(new ResponseApiModel<string>() { Status = EStatusCode.DATA_INVAILD, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.DATA_INVAILD) });

            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                string keyRedis = "CacheComment:" + barcode;
                string jsonListMail = RedisGatewayManager<string>.Inst.GetDataFromCache(keyRedis);
                if (string.IsNullOrEmpty(jsonListMail))
                {
                    var listComment =  StoreBookSqlInstance.Inst.GetListRateComment(barcode, out responseStatus);
                    if (responseStatus == EStatusCode.SUCCESS)
                    {
                        jsonListMail = JsonConvert.SerializeObject(listComment);
                        RedisGatewayManager<string>.Inst.SaveData(keyRedis, jsonListMail, 600);
                    }
                }
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = jsonListMail };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-GetListComment{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }
        [HttpGet]
        [Route("{barcode}/GetAvgRate")]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetAvgRate(string barcode)
        {
            if (string.IsNullOrEmpty(barcode))
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.DATA_INVAILD, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.DATA_INVAILD) });

            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            float starRate = 0;
            try
            {
                string keyRedis = "CacheAvgRate:" + barcode;
                string jsonListMail = RedisGatewayManager<string>.Inst.GetDataFromCache(keyRedis);
                if (string.IsNullOrEmpty(jsonListMail))
                {
                    responseStatus = StoreBookSqlInstance.Inst.GetAvgRate(barcode, out starRate);
                    if (responseStatus == EStatusCode.SUCCESS)
                    {
                        jsonListMail = JsonConvert.SerializeObject(starRate.ToString());
                        RedisGatewayManager<string>.Inst.SaveData(keyRedis, jsonListMail, 600);
                    }
                }
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = jsonListMail };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-GetAvgRate{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("{barcode}/SendCommentbook")]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> SendCommentbook(string barcode, int rate, string comment, string Nickname)
        {
            if (string.IsNullOrEmpty(barcode))
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.DATA_INVAILD, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.DATA_INVAILD) });

            long accountId = TokenManager.GetAccountIdByAccessToken(Request);
            if (accountId <= 0)
                return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });

            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            string jsonListMail = "";
            try
            {
                var commentObj = StoreBookSqlInstance.Inst.SendComment(accountId, barcode, rate, comment, UltilsHelper.FormatTime(DateTime.Now), Nickname, out responseStatus);
                if (responseStatus == EStatusCode.SUCCESS)
                {
                    string keyRedis = "CacheComment:" + barcode;
                    RedisGatewayManager<string>.Inst.DeleteDataFromCache(keyRedis);
                    jsonListMail = JsonConvert.SerializeObject(commentObj);
                }
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = jsonListMail };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-SendCommentbook{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }
        [HttpGet]
        [Route("GetListSimpleBook")]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetListSimpleBook(int page, int row)
        {
            if (row > 100)
                row = 100;

            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                string keyRedis = "CacheSimpleBook:" + page + "-"+row;
                string jsonListSimpleBook = RedisGatewayManager<string>.Inst.GetDataFromCache(keyRedis);
                if (string.IsNullOrEmpty(jsonListSimpleBook))
                {
                    var listBook = StoreBookSqlInstance.Inst.GetListSimpleBook(page, row, out responseStatus);
                    if (responseStatus == EStatusCode.SUCCESS)
                    {
                        jsonListSimpleBook = JsonConvert.SerializeObject(listBook);
                        RedisGatewayManager<string>.Inst.SaveData(keyRedis, jsonListSimpleBook, 600);
                    }
                }
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = jsonListSimpleBook };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-GetListSimpleBook{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("GetListSimpleBookSameName")]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetListSimpleBookSameName(string bookName)
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                string keyRedis = "CacheSimpleBook:" + bookName;
                string jsonListSimpleBook = RedisGatewayManager<string>.Inst.GetDataFromCache(keyRedis);
                if (string.IsNullOrEmpty(jsonListSimpleBook))
                {
                    var listBook = StoreBookSqlInstance.Inst.GetListSimpleBookByName(bookName, out responseStatus);
                    if (responseStatus == EStatusCode.SUCCESS)
                    {
                        jsonListSimpleBook = JsonConvert.SerializeObject(listBook);
                        RedisGatewayManager<string>.Inst.SaveData(keyRedis, jsonListSimpleBook, 600);
                    }
                }
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = jsonListSimpleBook };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-GetListSimpleBookSameName{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }
        [HttpGet]
        [Route("GetListSimpleBookbyTag")]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetListSimpleBookbyTag(string tag, int page, int row)
        {
            if (row > 100)
                row = 100;
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                string keyRedis = "CacheSimpleBook:" + page + "-" + row+"-"+ tag;
                string jsonListSimpleBook = RedisGatewayManager<string>.Inst.GetDataFromCache(keyRedis);
                if (string.IsNullOrEmpty(jsonListSimpleBook))
                {
                    var listBook = StoreBookSqlInstance.Inst.GetListSimpleBookByTag(tag, page, row, out responseStatus);
                    if (responseStatus == EStatusCode.SUCCESS)
                    {
                        jsonListSimpleBook = JsonConvert.SerializeObject(listBook);
                        RedisGatewayManager<string>.Inst.SaveData(keyRedis, jsonListSimpleBook, 600);
                    }
                }
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = jsonListSimpleBook };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-GetListSimpleBookbyTag{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }
    }
}
