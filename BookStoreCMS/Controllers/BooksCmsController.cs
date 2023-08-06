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
using ShareData.DB.Books;
using ShareData.ErrorCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UtilsSystem.Utils;

namespace BookStoreCMS.Controllers
{
    [ApiVersion("1.0")]
    [Route("v1/Books")]
    [ApiController]
    public class BooksCmsController : ControllerBase
    {

        private ILoggerManager _logger;
        private ITokenManager _tokenManager;
        public BooksCmsController(ILoggerManager logger, ITokenManager tokenManager)
        {
            _logger = logger;
            _tokenManager = tokenManager;
        }

        #region RATE
        [HttpGet]
        [Route("{barcode}/GetAvgRate")]
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
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = jsonListMail };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-GetAvgRate{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }


        [HttpPost]
        [Route("AddFakeRate")]
        public async Task<IActionResult> AddBook(RateCommentObject model)
        {
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                RateCountModel modelRate = null;
                var book = StoreBookSqlInstance.Inst.SendComment(model.AccountId,model.Barcode, model.StarRate, model.Comment, model.ActionTime, model.NickName, out responseStatus, out modelRate);
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = JsonConvert.SerializeObject(book) };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-UpdateAuthorConfig{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }
        #endregion

        #region COLOR
        [HttpPost]
        [Route("AddColorConfig")]
        public async Task<IActionResult> SendCommentbook(BookColorConfig data)
        {
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });

            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                responseStatus = StoreBookSqlInstance.Inst.AddColorConfig(data);
                if (responseStatus  == EStatusCode.SUCCESS) {
                    string keyRedis = "CacheListColorConfig";
                    await RedisGatewayCacheManager.Inst.DeleteDataFromCacheAsync(keyRedis);
                }
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus) };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-AddColor{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }
        [HttpPost]
        [Route("UpdateColorConfig")]
        public async Task<IActionResult> UpdateColorConfig(BookColorConfig data)
        {
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });

            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                responseStatus = StoreBookSqlInstance.Inst.UpdateColorConfig(data);
                if (responseStatus  == EStatusCode.SUCCESS) {
                    string keyRedis = "CacheListColorConfig";
                    await RedisGatewayCacheManager.Inst.DeleteDataFromCacheAsync(keyRedis);
                }
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus) };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-UpdateColorConfig{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("GetAllColorConfig")]
        public async Task<IActionResult> GetAllColorConfig()
        {
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });

            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            try
            {
                int responseStatus = EStatusCode.DATABASE_ERROR;
                var listColor = StoreBookSqlInstance.Inst.GetListColorConfigAll(out responseStatus);
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = JsonConvert.SerializeObject(listColor) };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-GetAllColorConfig{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }

        #endregion

        #region TAG
        [HttpPost]
        [Route("AddTag")]
        public async Task<IActionResult> AddTag(BookTagModel data)
        {
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });

            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                var tagModel = StoreBookSqlInstance.Inst.AddTagConfig(data, out responseStatus);
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus) , DataResponse = JsonConvert.SerializeObject(tagModel)};
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-AddColor{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }
        [HttpPost]
        [Route("UpdateTag")]
        public async Task<IActionResult> UpdateTag(BookTagModel data)
        {
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });

            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                var tagModel = StoreBookSqlInstance.Inst.UpdateTagConfig(data, out responseStatus);
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = JsonConvert.SerializeObject(tagModel) };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-AddColor{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("GetAllTag")]
        public async Task<IActionResult> GetAllTag()
        {
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });

            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                var listTag = StoreBookSqlInstance.Inst.GetAllTagConfig(out responseStatus);
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = JsonConvert.SerializeObject(listTag) };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-GetAllTag{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }
        #endregion

        #region AUTHOR
        [HttpGet]
        [Route("GetlistAuthorConfig")]
        public async Task<IActionResult> GetListColorConfig()
        {
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                var listAuthor = StoreBookSqlInstance.Inst.GetListAuthorConfigAll(out responseStatus);
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = JsonConvert.SerializeObject(listAuthor) };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-GetlistAuthorConfig{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("AddAuthorConfig")]
        public async Task<IActionResult> AddAuthorConfig(AuthorInfoModel model)
        {
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                var author = StoreBookSqlInstance.Inst.AddNewAuthor(model, out responseStatus);
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = JsonConvert.SerializeObject(author) };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-AddAuthorConfig{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("UpdateAuthorConfig")]
        public async Task<IActionResult> UpdateAuthorConfig(AuthorInfoModel model)
        {
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                responseStatus = StoreBookSqlInstance.Inst.UpdateAuthorConfig(model);
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus) };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-UpdateAuthorConfig{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }
        #endregion

        #region BOOK

        [HttpPost]
        [Route("AddBook")]
        public async Task<IActionResult> AddBook(AddBookModel model)
        {
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                var book = StoreBookSqlInstance.Inst.AddNewBook(model, out responseStatus);
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = JsonConvert.SerializeObject(book) };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-AddBook{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("UpdateBook")]
        public async Task<IActionResult> UpdateBook(AddBookModel model)
        {
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                var book = StoreBookSqlInstance.Inst.UpdateBook(model, out responseStatus);
                if (responseStatus == EStatusCode.SUCCESS)
                {
                    await RedisGatewayCacheManager.Inst.DeleteDataFromCacheAsync("CacheSimpleBookDemo:" + model.Barcode);
                    await RedisGatewayCacheManager.Inst.DeleteDataFromCacheAsync("CacheSimpleBook:" + model.Barcode);
                }
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = JsonConvert.SerializeObject(book) };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-UpdateBook{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("GetBook")]
        public async Task<IActionResult> GetBook(string barcode)
        {
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                var book = StoreBookSqlInstance.Inst.GetBookCmsByBarcode(barcode, out responseStatus);
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = JsonConvert.SerializeObject(book) };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-GetBook{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }
        [HttpGet]
        [Route("GetBooks")]
        public async Task<IActionResult> GetBooks(int page, int row)
        {
            if (page > 100)
                page = 100;
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                var book = StoreBookSqlInstance.Inst.GetListBookCms(page, row, out responseStatus);
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = JsonConvert.SerializeObject(book) };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-GetBooks{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }
        #endregion

        #region Feature book 
        [HttpGet]
        [Route("GetFeaturedBookConfig")]
        public async Task<IActionResult> GetFeaturedBookConfig()
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                var bookConfig = StoreBookSqlInstance.Inst.GetFeatureBookConfigAll(out responseStatus);
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = JsonConvert.SerializeObject(bookConfig) };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-UpdateAuthorConfig{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }
        [HttpPost]
        [Route("AddFeaturedBookConfig")]
        public async Task<IActionResult> AddFeaturedBookConfig(FeaturedBookConfigModel model)
        {
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                var bookConfig = StoreBookSqlInstance.Inst.AddFeatureBookConfig(model, out responseStatus);
                if (responseStatus == EStatusCode.SUCCESS)
                {
                    string keyRedis = "FeaturedBookConfig";
                    await RedisGatewayCacheManager.Inst.DeleteDataFromCacheAsync(keyRedis);
                }
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = JsonConvert.SerializeObject(bookConfig) };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-AddFeaturedBookConfig{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("UpdateFeaturedBookConfig")]
        public async Task<IActionResult> UpdateFeaturedBookConfig(FeaturedBookConfigModel model)
        {
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                var book = StoreBookSqlInstance.Inst.UpdateFeatureBookConfig(model, out responseStatus);
                if (responseStatus == EStatusCode.SUCCESS)
                {
                    string keyRedis = "FeaturedBookConfig";
                    await RedisGatewayCacheManager.Inst.DeleteDataFromCacheAsync(keyRedis);
                }
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = JsonConvert.SerializeObject(book) };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-UpdateFeaturedBookConfig{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("GetFeaturedBookData")]
        //[ResponseCache(Duration = 10)]
        public async Task<IActionResult> GetFeaturedBookData(int FeatureType)
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                var bookData = StoreBookSqlInstance.Inst.GetFeatureBookData(FeatureType, out responseStatus);
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = JsonConvert.SerializeObject(bookData) };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-GetFeaturedBookData{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("AddFeaturedBookData")]
        public async Task<IActionResult> AddFeaturedBookData(int FeatureType, string Barcode)
        {
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                var bookConfig = StoreBookSqlInstance.Inst.AddFeatureBookData(FeatureType, Barcode, out responseStatus);
                if (responseStatus == EStatusCode.SUCCESS)
                {
                    string keyRedis = "GetFeaturedBookData:" + FeatureType;
                    await RedisGatewayCacheManager.Inst.DeleteArrayKeyAsync(keyRedis);
                }
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = JsonConvert.SerializeObject(bookConfig) };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-AddFeaturedBookConfig{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("RemoveFeaturedBookData")]
        public async Task<IActionResult> RemoveFeaturedBookData(FeaturedBookDataModel model)
        {
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            try
            {
                int responseStatus = StoreBookSqlInstance.Inst.RemoveFeatureBookData(model);
                if (responseStatus == EStatusCode.SUCCESS)
                {
                    string keyRedis = "GetFeaturedBookData:" + model.FeatureType;
                    await RedisGatewayCacheManager.Inst.DeleteArrayKeyAsync(keyRedis);
                }
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus)};
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-RemoveFeaturedBookData{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }
        #endregion
        #region Reprint book 
        [HttpGet]
        [Route("GetReprintBook")]
        public async Task<IActionResult> GetReprintBook()
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                var bookConfig = StoreBookSqlInstance.Inst.GetAllReprintBook(out responseStatus);
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = JsonConvert.SerializeObject(bookConfig) };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-GetReprintBook{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }
        [HttpPost]
        [Route("AddReprintBook")]
        public async Task<IActionResult> AddReprintBook(string barcode)
        {
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                StoreBookSqlInstance.Inst.AddReprintBook(barcode, out responseStatus);
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus) };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-AddReprintBook{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("RemoveReprintBook")]
        public async Task<IActionResult> RemoveReprintBook(string barcode)
        {
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                StoreBookSqlInstance.Inst.RemoveReprintBook(barcode, out responseStatus);
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus) };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-RemoveReprintBook{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }
        #endregion
    }
}
