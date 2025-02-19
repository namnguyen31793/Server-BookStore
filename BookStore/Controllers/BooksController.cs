﻿using BookStore.Interfaces;
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
using ShareData.Response;
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
        [ResponseCache(Duration = 10)]
        public async Task<IActionResult> GetListComment(string barcode)
        {
            if(string.IsNullOrEmpty(barcode))
            return Ok(new ResponseApiModel<string>() { Status = EStatusCode.DATA_INVAILD, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.DATA_INVAILD) });

            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;

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

            return Ok(response);
        }

        [HttpGet]
        [Route("{barcode}/GetComment")]
        [ResponseCache(Duration = 10)]
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

            var listComment = StoreBookSqlInstance.Inst.GetRateComment(accountId, barcode, out responseStatus);
            jsonListMail = JsonConvert.SerializeObject(listComment);
            responseStatus = EStatusCode.SUCCESS;
            response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = jsonListMail };

            return Ok(response);
        }

        [HttpGet]
        [Route("{barcode}/GetAvgRate")]
        [ResponseCache(Duration = 10)]
        public async Task<IActionResult> GetAvgRate(string barcode)
        {
            if (string.IsNullOrEmpty(barcode))
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.DATA_INVAILD, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.DATA_INVAILD) });

            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            string jsonListMail = "";

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

            return Ok(response);
        }

        [HttpPost]
        [Route("{barcode}/SendCommentbook")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
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
 
            ResponseSendComment responseModel = null;
            RateCountModel modelRate = null;
            var commentObj = StoreBookSqlInstance.Inst.SendComment(accountId, barcode, data.Rate, data.Comment, UltilsHelper.FormatTime(DateTime.Now), data.Nickname, out responseStatus, out modelRate);
            await _logger.LogInfo("Books-SendCommentbook{}", barcode + " - accountId:" + accountId + " - " + data.Comment + " - " + data.Nickname + " - " + responseStatus, responseStatus.ToString()).ConfigureAwait(false);
            if (responseStatus == EStatusCode.SUCCESS)
            {
                responseModel = new ResponseSendComment() { comment = commentObj, rateStar = modelRate };

                string keyRedis = "CacheComment:" + barcode;
                await RedisGatewayCacheManager.Inst.DeleteDataFromCacheAsync(keyRedis).ConfigureAwait(false);
                keyRedis = "CacheAvgRate:" + barcode;
                await RedisGatewayCacheManager.Inst.SaveDataAsync(keyRedis, JsonConvert.SerializeObject(modelRate), 10);

                jsonListMail = JsonConvert.SerializeObject(responseModel);
            }
            response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = jsonListMail };

            return Ok(response);
        }

        [HttpGet]
        [Route("{barcode}/GetListSimpleBook")]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetListSimpleBookBarCode(string barcode)
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;

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

            return Ok(response);
        }
        [HttpGet]
        [Route("{barcode}/GetBookDemo")]
        [ResponseCache(Duration = 10)]
        public async Task<IActionResult> GetBookDemo(string barcode)
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
           
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

            return Ok(response);
        }

        [HttpGet]
        [Route("{barcode}/GetLinkDownload")]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetLinkDownload(string barcode)
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;

            long accountId = await _tokenManager.GetAccountIdByAccessTokenAsync(Request);
            if (accountId <= 0)
                return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });

            string ìnoDownload = "";
            var downloadInfoModel = StoreBookSqlInstance.Inst.GetDownloadBookByBarcode(accountId, barcode, out responseStatus);
            if (responseStatus == EStatusCode.SUCCESS)
            {
                ìnoDownload = JsonConvert.SerializeObject(downloadInfoModel);
            }
            response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = ìnoDownload };

            return Ok(response);
        }

        [HttpGet]
        [Route("{barcode}/GetLinkDownloadV2")]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetLinkDownloadV2(string barcode)
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;

            string ìnoDownload = "";
            var downloadInfoModel = StoreBookSqlInstance.Inst.GetDownloadBookByBarcode(barcode, out responseStatus);
            if (responseStatus == EStatusCode.SUCCESS)
            {
                ìnoDownload = JsonConvert.SerializeObject(downloadInfoModel);
            }
            response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = ìnoDownload };

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

            return Ok(response);
        }

        [HttpGet]
        [Route("GetListSimpleBookSameName")]
        public async Task<IActionResult> GetListSimpleBookSameName(string bookName)
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;

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

            return Ok(response);
        }

        [HttpGet]
        [Route("GetAllTag")]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetAllTag()
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
 
            string keyRedis = "CacheAllTag";
            string jsonListSimpleBook = await RedisGatewayCacheManager.Inst.GetDataFromCacheAsync(keyRedis);
            if (string.IsNullOrEmpty(jsonListSimpleBook))
            {
                var listBook = StoreBookSqlInstance.Inst.GetAllTagConfig(out responseStatus);
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

            return Ok(response);
        }
        [HttpGet]
        [Route("GetListColorConfig")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> GetListColorConfig()
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;

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

            return Ok(response);
        }

        [HttpPost]
        [Route("{barcode}/LikeBook")]
        [ResponseCache(Duration = 60)]
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

            return Ok(response);
        }

        [HttpGet]
        [Route("{TagId}/GetTagConfig")]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetTagConfig2(int TagId)
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;

            string keyRedis = "TagConfig:" + TagId;
            string jsonTag = await RedisGatewayCacheManager.Inst.GetDataFromCacheAsync(keyRedis);
            if (string.IsNullOrEmpty(jsonTag))
            {
                var tagModel = StoreBookSqlInstance.Inst.GetTagConfig(TagId, out responseStatus);
                if (responseStatus == EStatusCode.SUCCESS)
                {
                    jsonTag = JsonConvert.SerializeObject(tagModel);
                    await RedisGatewayCacheManager.Inst.SaveDataAsync(keyRedis, jsonTag, 5);
                }
            }
            else
            {
                responseStatus = EStatusCode.SUCCESS;
            }
            response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = jsonTag };

            return Ok(response);
        }

        [HttpGet]
        [Route("GetTagConfig")]
        public async Task<IActionResult> GetTagConfig(int TagId)
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;

            string keyRedis = "TagConfig:"+ TagId;
            string jsonTag = await RedisGatewayCacheManager.Inst.GetDataFromCacheAsync(keyRedis);
            if (string.IsNullOrEmpty(jsonTag))
            {
                var tagModel = StoreBookSqlInstance.Inst.GetTagConfig(TagId, out responseStatus);
                if (responseStatus == EStatusCode.SUCCESS)
                {
                    jsonTag = JsonConvert.SerializeObject(tagModel);
                    await RedisGatewayCacheManager.Inst.SaveDataAsync(keyRedis, jsonTag, 5);
                }
            }
            else
            {
                responseStatus = EStatusCode.SUCCESS;
            }
            response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = jsonTag };

            return Ok(response);
        }

        #region Feature book 
        [HttpGet]
        [Route("GetFeaturedBookConfig")]
        //[ResponseCache(Duration = 10)]
        public async Task<IActionResult> GetFeaturedBookConfig()
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;

            string keyRedis = "FeaturedBookConfig" ;
            string jsonTag = await RedisGatewayCacheManager.Inst.GetDataFromCacheAsync(keyRedis);
            if (string.IsNullOrEmpty(jsonTag))
            {
                var bookConfig = StoreBookSqlInstance.Inst.GetFeatureBookConfig(out responseStatus);
                if (responseStatus == EStatusCode.SUCCESS)
                {
                    jsonTag = JsonConvert.SerializeObject(bookConfig);
                    await RedisGatewayCacheManager.Inst.SaveDataAsync(keyRedis, jsonTag, 5);
                }
            }
            else
            {
                responseStatus = EStatusCode.SUCCESS;
            }
            response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = jsonTag };

            return Ok(response);
        }

        [HttpGet]
        [Route("GetFeaturedBookData")]
        //[ResponseCache(Duration = 10)]
        public async Task<IActionResult> GetFeaturedBookData(int FeatureType)
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;

            string keyRedis = "GetFeaturedBookData:"+ FeatureType;
            string jsonTag = await RedisGatewayCacheManager.Inst.GetDataFromCacheAsync(keyRedis);
            if (string.IsNullOrEmpty(jsonTag))
            {
                var bookData = StoreBookSqlInstance.Inst.GetFeatureBookData(FeatureType, out responseStatus);
                if (responseStatus == EStatusCode.SUCCESS)
                {
                    jsonTag = JsonConvert.SerializeObject(bookData);
                    await RedisGatewayCacheManager.Inst.SaveDataAsync(keyRedis, jsonTag, 5);
                }
            }
            else
            {
                responseStatus = EStatusCode.SUCCESS;
            }
            response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = jsonTag };

            return Ok(response);
        }

        #endregion

        #region REPRINT

        [HttpGet]
        [Route("{barcode}/CheckReprintBook")]
        [ResponseCache(Duration = 10)]
        public async Task<IActionResult> CheckReprintBook(string barcode)
        {
            var response = new ResponseApiModel<bool>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.SUCCESS;

            var bookData = StoreBookSqlInstance.Inst.CheckReprintBook(barcode);
            response = new ResponseApiModel<bool>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = bookData };

            return Ok(response);
        }

        [HttpGet]
        [Route("GetReprintBook")]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetReprintBook()
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;

            var bookConfig = StoreBookSqlInstance.Inst.GetAllReprintBook(out responseStatus);
            response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = JsonConvert.SerializeObject(bookConfig) };

            return Ok(response);
        }
        #endregion
    }
}
