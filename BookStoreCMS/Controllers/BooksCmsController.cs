﻿using BookStoreCMS.Utils;
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
        public BooksCmsController(ILoggerManager logger)
        {
            _logger = logger;
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
        #region COLOR
        [HttpPost]
        [Route("AddColorConfig")]
        public async Task<IActionResult> SendCommentbook(BookColorConfig data)
        {
            int checkRole = TokenCMSManager.CheckRoleAction(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });

            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                responseStatus = StoreBookSqlInstance.Inst.AddColorConfig(data);
                if (responseStatus  == EStatusCode.SUCCESS) {
                    string keyRedis = "CacheListColorConfig";
                    RedisGatewayManager<string>.Inst.DeleteDataFromCache(keyRedis);
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
            int checkRole = TokenCMSManager.CheckRoleAction(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });

            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                responseStatus = StoreBookSqlInstance.Inst.UpdateColorConfig(data);
                if (responseStatus  == EStatusCode.SUCCESS) {
                    string keyRedis = "CacheListColorConfig";
                    RedisGatewayManager<string>.Inst.DeleteDataFromCache(keyRedis);
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
        [ResponseCache(Duration = 5)]
        public async Task<IActionResult> GetAllColorConfig()
        {
            int checkRole = TokenCMSManager.CheckRoleAction(ERole.Administrator, Request);
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
            int checkRole = TokenCMSManager.CheckRoleAction(ERole.Administrator, Request);
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
            int checkRole = TokenCMSManager.CheckRoleAction(ERole.Administrator, Request);
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
        [ResponseCache(Duration = 5)]
        public async Task<IActionResult> GetAllTag()
        {
            int checkRole = TokenCMSManager.CheckRoleAction(ERole.Administrator, Request);
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
        [ResponseCache(Duration = 5)]
        public async Task<IActionResult> GetListColorConfig()
        {
            int checkRole = TokenCMSManager.CheckRoleAction(ERole.Administrator, Request);
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
            int checkRole = TokenCMSManager.CheckRoleAction(ERole.Administrator, Request);
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
            int checkRole = TokenCMSManager.CheckRoleAction(ERole.Administrator, Request);
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
            int checkRole = TokenCMSManager.CheckRoleAction(ERole.Administrator, Request);
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
                await _logger.LogError("Books-UpdateAuthorConfig{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("UpdateBook")]
        public async Task<IActionResult> UpdateBook(AddBookModel model)
        {
            int checkRole = TokenCMSManager.CheckRoleAction(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                var book = StoreBookSqlInstance.Inst.UpdateBook(model, out responseStatus);
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = JsonConvert.SerializeObject(book) };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-UpdateAuthorConfig{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("GetBook")]
        [ResponseCache(Duration = 5)]
        public async Task<IActionResult> GetBook(string barcode)
        {
            int checkRole = TokenCMSManager.CheckRoleAction(ERole.Administrator, Request);
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
                await _logger.LogError("Books-UpdateAuthorConfig{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }
        [HttpGet]
        [Route("GetBooks")]
        [ResponseCache(Duration = 5)]
        public async Task<IActionResult> GetBooks(int page, int row)
        {
            if (page > 100)
                page = 100;
            int checkRole = TokenCMSManager.CheckRoleAction(ERole.Administrator, Request);
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
                await _logger.LogError("Books-UpdateAuthorConfig{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }
        #endregion
    }
}