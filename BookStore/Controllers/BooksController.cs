using BookStore.Interfaces;
using BookStore.Utils;
using DAO.DAOImp;
using LoggerService;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RedisSystem;
using ShareData.API;
using ShareData.DB.Books;
using ShareData.ErrorCode;
using ShareData.Request;
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
        private ITokenManager _tokenManager;
        public BooksController(ILoggerManager logger, ITokenManager tokenManager)
        {
            _logger = logger;
            _tokenManager = tokenManager;
        }

        [HttpGet]
        [Route("{barcode}/GetListComment")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        [HttpCacheValidation(MustRevalidate = true)]
        public async Task<IActionResult> GetListComment(string barcode)
        {
            if(string.IsNullOrEmpty(barcode))
            return Ok(new ResponseApiModel<string>() { Status = EStatusCode.DATA_INVAILD, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.DATA_INVAILD) });

            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                string keyRedis = "CacheComment:" + barcode;
                string jsonListMail = await RedisGatewayCacheManager.Inst.GetDataFromCacheAsync(keyRedis);
                if (string.IsNullOrEmpty(jsonListMail))
                {
                    var listComment =  StoreBookSqlInstance.Inst.GetListRateComment(barcode, out responseStatus);
                    if (responseStatus == EStatusCode.SUCCESS)
                    {
                        jsonListMail = JsonConvert.SerializeObject(listComment);
                        await RedisGatewayCacheManager.Inst.SaveDataAsync(keyRedis, jsonListMail, 10);
                    }
                }
                else
                {
                    responseStatus = EStatusCode.SUCCESS;
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
        [Route("{barcode}/GetComment")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 10)]
        [HttpCacheValidation(MustRevalidate = true)]
        public async Task<IActionResult> GetComment(string barcode)
        {
            if (string.IsNullOrEmpty(barcode))
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.DATA_INVAILD, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.DATA_INVAILD) });
            long accountId = await _tokenManager.GetAccountIdByAccessTokenAsync(Request);
            if (accountId <= 0)
                return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            string jsonListMail = "";
            try
            {
                var listComment = StoreBookSqlInstance.Inst.GetRateComment(accountId, barcode, out responseStatus);
                jsonListMail = JsonConvert.SerializeObject(listComment);
                responseStatus = EStatusCode.SUCCESS;
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = jsonListMail };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-GetComment{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("{barcode}/GetAvgRate")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 10)]
        [HttpCacheValidation(MustRevalidate = true)]
        public async Task<IActionResult> GetAvgRate(string barcode)
        {
            if (string.IsNullOrEmpty(barcode))
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.DATA_INVAILD, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.DATA_INVAILD) });

            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            string jsonListMail = "";
            try
            {
                string keyRedis = "CacheAvgRate:" + barcode;
                jsonListMail = await RedisGatewayCacheManager.Inst.GetDataFromCacheAsync(keyRedis);
                if (string.IsNullOrEmpty(jsonListMail))
                {
                    RateCountModel starRate = null;
                    responseStatus = StoreBookSqlInstance.Inst.GetAvgRate(barcode, out starRate);
                    if (responseStatus == EStatusCode.SUCCESS)
                    {
                        jsonListMail = JsonConvert.SerializeObject(starRate);
                        await RedisGatewayCacheManager.Inst.SaveDataAsync(keyRedis, jsonListMail, 10);
                    }
                }
                else
                {
                    responseStatus = EStatusCode.SUCCESS;
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
        [HttpCacheIgnore]
        public async Task<IActionResult> SendCommentbook(string barcode, [FromBody]RequestCommendModel data)
        {
            if (string.IsNullOrEmpty(barcode))
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.DATA_INVAILD, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.DATA_INVAILD) });

            long accountId = await _tokenManager.GetAccountIdByAccessTokenAsync(Request);
            if (accountId <= 0)
                return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });

            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            string jsonListMail = "";
            try
            {
                var commentObj = StoreBookSqlInstance.Inst.SendComment(accountId, barcode, data.Rate, data.Comment, UltilsHelper.FormatTime(DateTime.Now), data.Nickname, out responseStatus);
                await _logger.LogInfo("Books-SendCommentbook{}", barcode + " - accountId:" + accountId + " - " + data.Comment + " - " + data.Nickname + " - " + responseStatus, responseStatus.ToString()).ConfigureAwait(false);
                if (responseStatus == EStatusCode.SUCCESS)
                {
                    string keyRedis = "CacheComment:" + barcode;
                    await RedisGatewayCacheManager.Inst.DeleteDataFromCacheAsync(keyRedis).ConfigureAwait(false);
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
        [Route("{barcode}/GetListSimpleBook")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 10)]
        [HttpCacheValidation(MustRevalidate = true)]
        public async Task<IActionResult> GetListSimpleBookBarCode(string barcode)
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                string keyRedis = "CacheSimpleBook:" + barcode;
                string jsonListSimpleBook = await RedisGatewayCacheManager.Inst.GetDataFromCacheAsync(keyRedis);
                if (string.IsNullOrEmpty(jsonListSimpleBook))
                {
                    var listBook = StoreBookSqlInstance.Inst.GetListSimpleBookByBarcode(barcode, out responseStatus);
                    if (responseStatus == EStatusCode.SUCCESS)
                    {
                        jsonListSimpleBook = JsonConvert.SerializeObject(listBook);
                        await RedisGatewayCacheManager.Inst.SaveDataAsync(keyRedis, jsonListSimpleBook, 10);
                    }
                }
                else {
                    responseStatus = EStatusCode.SUCCESS;
                }
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = jsonListSimpleBook };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-GetListSimpleBookbyTag{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }
        [HttpGet]
        [Route("{barcode}/GetBookDemo")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        [HttpCacheValidation(MustRevalidate = true)]
        public async Task<IActionResult> GetBookDemo(string barcode)
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                string keyRedis = "CacheSimpleBookDemo:" + barcode;
                string jsonListSimpleBook = await RedisGatewayCacheManager.Inst.GetDataFromCacheAsync(keyRedis);
                if (string.IsNullOrEmpty(jsonListSimpleBook))
                {
                    var listBook = StoreBookSqlInstance.Inst.GetFullBookDemoByBarcode(barcode, out responseStatus);
                    if (responseStatus == EStatusCode.SUCCESS)
                    {
                        jsonListSimpleBook = JsonConvert.SerializeObject(listBook);
                        await RedisGatewayCacheManager.Inst.SaveDataAsync(keyRedis, jsonListSimpleBook, 10);
                    }
                }
                else {
                    responseStatus = EStatusCode.SUCCESS;
                }
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = jsonListSimpleBook };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-GetListSimpleBookbyTag{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("{barcode}/GetLinkDownload")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 10)]
        [HttpCacheValidation(MustRevalidate = true)]
        public async Task<IActionResult> GetLinkDownload(string barcode)
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;

            long accountId = await _tokenManager.GetAccountIdByAccessTokenAsync(Request);
            if (accountId <= 0)
                return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });
            try
            {
                string ìnoDownload = "";
                var downloadInfoModel = StoreBookSqlInstance.Inst.GetDownloadBookByBarcode(accountId, barcode, out responseStatus);
                if (responseStatus == EStatusCode.SUCCESS)
                {
                    ìnoDownload = JsonConvert.SerializeObject(downloadInfoModel);
                }
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = ìnoDownload };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-GetLinkDownload{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("GetListSimpleBook")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 10)]
        [HttpCacheValidation(MustRevalidate = true)]
        public async Task<IActionResult> GetListSimpleBook(int page, int row)
        {
            if (row > 100)
                row = 100;

            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                string keyRedis = "CacheSimpleBook:" + page + "-"+row;
                string jsonListSimpleBook = await RedisGatewayCacheManager.Inst.GetDataFromCacheAsync(keyRedis);
                if (string.IsNullOrEmpty(jsonListSimpleBook))
                {
                    var listBook = StoreBookSqlInstance.Inst.GetListSimpleBook(page, row, out responseStatus);
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
                await _logger.LogError("Books-GetListSimpleBook{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("GetListSimpleBookSameName")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 10)]
        [HttpCacheValidation(MustRevalidate = true)]
        public async Task<IActionResult> GetListSimpleBookSameName(string bookName)
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                string keyRedis = "CacheSimpleBook:" + bookName;
                string jsonListSimpleBook = await RedisGatewayCacheManager.Inst.GetDataFromCacheAsync(keyRedis);
                if (string.IsNullOrEmpty(jsonListSimpleBook))
                {
                    var listBook = StoreBookSqlInstance.Inst.GetListSimpleBookByName(bookName, out responseStatus);
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
                await _logger.LogError("Books-GetListSimpleBookSameName{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }
        [HttpGet]
        [Route("GetListSimpleBookbyTag")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 10)]
        [HttpCacheValidation(MustRevalidate = true)]
        public async Task<IActionResult> GetListSimpleBookbyTag(string tag, int page, int row)
        {
            if (row > 100)
                row = 100;
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                string keyRedis = "CacheSimpleBook:" + page + "-" + row+"-"+ tag;
                string jsonListSimpleBook = await RedisGatewayCacheManager.Inst.GetDataFromCacheAsync(keyRedis);
                if (string.IsNullOrEmpty(jsonListSimpleBook))
                {
                    var listBook = StoreBookSqlInstance.Inst.GetListSimpleBookByTag(tag, page, row, out responseStatus);
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
                await _logger.LogError("Books-GetListSimpleBookbyTag{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }
        [HttpGet]
        [Route("GetListColorConfig")]
        [HttpCacheIgnore]
        public async Task<IActionResult> GetListColorConfig()
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                string keyRedis = "CacheListColorConfig" ;
                string jsonListSimpleBook = await RedisGatewayCacheManager.Inst.GetDataFromCacheAsync(keyRedis);
                if (string.IsNullOrEmpty(jsonListSimpleBook))
                {
                    var listBook = StoreBookSqlInstance.Inst.GetListColorConfig(out responseStatus);
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
                await _logger.LogError("Books-GetListSimpleBookSameName{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("{barcode}/LikeBook")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 10)]
        [HttpCacheValidation(MustRevalidate = true)]
        public async Task<IActionResult> LikeBook(string barcode, int status = 0)
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            bool like = false;
            if (status == 1)
                like = true;
            long accountId = await _tokenManager.GetAccountIdByAccessTokenAsync(Request);
            if (accountId <= 0)
                return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });
            try
            {
                var downloadInfoModel = StoreBookSqlInstance.Inst.LikeBook(accountId, barcode, like, UltilsHelper.FormatTime(DateTime.Now), out responseStatus);
                await _logger.LogInfo("Books-LikeBook{}", barcode + " - accountId:" + accountId + " - " + responseStatus, responseStatus.ToString()).ConfigureAwait(false);
                if (responseStatus == EStatusCode.SUCCESS)
                {
                    string keyRedis = "CacheLikeBook:" + accountId;
                    await RedisGatewayCacheManager.Inst.DeleteArrayKeyAsync(keyRedis).ConfigureAwait(false);
                    keyRedis = "CacheCountLikeBook:" + accountId;
                    await RedisGatewayCacheManager.Inst.DeleteDataFromCacheAsync(keyRedis).ConfigureAwait(false);
                }
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus) };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-LikeBook{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }
    }
}
