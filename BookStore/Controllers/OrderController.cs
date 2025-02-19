﻿using BookStore.Instance;
using BookStore.Interfaces;
using BookStore.Utils;
using DAO.DAOImp;
using LoggerService;
using LoggerService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RedisSystem;
using ShareData.API;
using ShareData.DB.Order;
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
    [Route("v1/Orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private IReportLog _report;
        private ILoggerManager _logger;
        private IEmailSender _emailSender;
        private ITokenManager _tokenManager; 
        public OrderController(ILoggerManager logger, IEmailSender emailSender, ITokenManager tokenManager, IReportLog reportManager)
        {
            _logger = logger;
            _emailSender = emailSender;
            _tokenManager = tokenManager;
            _report = reportManager;
        }
        private string token = string.Empty;

        #region INFO SHIP
        [HttpPost]
        [Route("CreateOrderInfo")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> CreateOrderInfo(RequestCreateOrderInfoModel request)
        {
            int responseStatus = -99;
            var clientInfo = new ClientRequestInfo(Request);
            string key = "SPAM:CREATE_ORDER_INFO" + clientInfo.TrueClientIp + "-" + clientInfo.DeviceId;
            if (RedisGatewayCacheManager.Inst.CheckExistKey(key))
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.TRANSACTION_SPAM, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.TRANSACTION_SPAM) });
            long accountId = await _tokenManager.GetAccountIdByAccessTokenAsync(Request);
            if (accountId <= 0)
                return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });
            await RedisGatewayCacheManager.Inst.SaveDataSecond(key, "1", 3).ConfigureAwait(false);

            var data = StoreOrderSqlInstance.Inst.CreateCustomerInfo(accountId, request.CustomerName, request.CustomerMobile, request.CustomerEmail, request.CustomerAddress, request.Defaut, out responseStatus);
            //update cache
            if (responseStatus == EStatusCode.SUCCESS) {
                string keyRedis = "CUSTOMER:" + accountId;
                await RedisGatewayCacheManager.Inst.SaveDataSecond(keyRedis, JsonConvert.SerializeObject(data), 300).ConfigureAwait(false);
            }
            return Ok(new ResponseApiModel<List<CustomerInfoModel>>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = data });
        }

        [HttpPost]
        [Route("UpdateOrderInfo")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> UpdateOrderInfo(RequestUpdaterOrderInfoModel request)
        {
            int responseStatus = -99;
            var clientInfo = new ClientRequestInfo(Request);
            string key = "SPAM:UPDATE_ORDER_INFO" + clientInfo.TrueClientIp + "-" + clientInfo.DeviceId;
            if (RedisGatewayCacheManager.Inst.CheckExistKey(key))
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.TRANSACTION_SPAM, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.TRANSACTION_SPAM) });
            long accountId = await _tokenManager.GetAccountIdByAccessTokenAsync(Request);
            if (accountId <= 0)
                return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });
            await RedisGatewayCacheManager.Inst.SaveDataSecond(key, "1", 3).ConfigureAwait(false);
            if (request.CustomerId <= 0)
                return Ok(new ResponseApiModel<List<CustomerInfoModel>>() { Status = EStatusCode.DATA_INVAILD, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.DATA_INVAILD) });

            var data = StoreOrderSqlInstance.Inst.UpdateCustomerInfo(request.CustomerId, request.CustomerName, request.CustomerMobile, request.CustomerEmail, request.CustomerAddress, request.Defaut, out responseStatus);
            //update cache
            if (responseStatus == EStatusCode.SUCCESS)
            {
                string keyRedis = "CUSTOMER:" + accountId;
                await RedisGatewayCacheManager.Inst.SaveDataSecond(keyRedis, JsonConvert.SerializeObject(data), 300).ConfigureAwait(false);
            }
            return Ok(new ResponseApiModel<List<CustomerInfoModel>>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = data });
        }
        [HttpPost]
        [Route("DeleteOrderInfo")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> DeleteOrderInfo(long CustomerId)
        {
            int responseStatus = -99;
          
            long accountId = await _tokenManager.GetAccountIdByAccessTokenAsync(Request);
            if (accountId <= 0)
                return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });

            var data = StoreOrderSqlInstance.Inst.DeleteCustomerInfo(CustomerId, out responseStatus);
            //update cache
            if (responseStatus == EStatusCode.SUCCESS)
            {
                string keyRedis = "CUSTOMER:" + accountId;
                await RedisGatewayCacheManager.Inst.SaveDataSecond(keyRedis, JsonConvert.SerializeObject(data), 300).ConfigureAwait(false);
            }
            return Ok(new ResponseApiModel<List<CustomerInfoModel>>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = data });
        }

        [HttpGet]
        [Route("GetOrderInfo")]
        [ResponseCache(Duration = 5)]
        public async Task<IActionResult> GetOrderInfo()
        {
            long accountId = await _tokenManager.GetAccountIdByAccessTokenAsync(Request);
            if (accountId <= 0)
                return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });
            int responseStatus = EStatusCode.DATABASE_ERROR;
            string keyRedis = "CUSTOMER:" + accountId;
            string jsonTag = await RedisGatewayCacheManager.Inst.GetDataFromCacheAsync(keyRedis);
            try {
                if (string.IsNullOrEmpty(jsonTag))
                {
                    var data = StoreOrderSqlInstance.Inst.GetCustomerInfo(accountId, out responseStatus);
                    //update cache
                    if (responseStatus == EStatusCode.SUCCESS)
                    {
                        jsonTag = JsonConvert.SerializeObject(data);
                        await RedisGatewayCacheManager.Inst.SaveDataSecond(keyRedis, jsonTag, 300).ConfigureAwait(false);
                    }
                }
                else
                {
                    responseStatus = EStatusCode.SUCCESS;
                }
            }
            catch { 
            
            }
            return Ok(new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = jsonTag });
        }
        #endregion

        #region ORDER GHTK
        [HttpPost]
        [Route("CreateOrder")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> CreateOrder(RequestCreateOrderModel request)
        {
            int responseStatus = -99;
            var clientInfo = new ClientRequestInfo(Request);
            string key = "SPAM:CREATE_ORDER" + clientInfo.TrueClientIp + "-" + clientInfo.DeviceId;
            if (RedisGatewayCacheManager.Inst.CheckExistKey(key))
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.TRANSACTION_SPAM, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.TRANSACTION_SPAM) });
            long accountId = await _tokenManager.GetAccountIdByAccessTokenAsync(Request);
            if (accountId <= 0)
                return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });
            await RedisGatewayCacheManager.Inst.SaveDataSecond(key, "1", 3).ConfigureAwait(false);
            //check barcode
            if (string.IsNullOrEmpty(request.Barcodes) || string.IsNullOrEmpty(request.Numbers))
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.ORDER_NOT_DATA, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.ORDER_NOT_DATA) });
            var dataBook = request.Barcodes.Split(",");
            var dataNumber = request.Numbers.Split(",");
            if (dataBook.Length != dataNumber.Length)
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.ORDER_NOT_DATA, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.ORDER_NOT_DATA) });
            try
            {
                for (int i = 0; i < dataNumber.Length; i++)
                {
                    var check = int.Parse(dataNumber[i]);
                }
            }
            catch
            {
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.ORDER_NOT_DATA, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.ORDER_NOT_DATA) });
            }
            var data = StoreOrderSqlInstance.Inst.CreateNewOrder(accountId, request.CustomerId, request.Type, request.Description, request.Barcodes, request.Numbers, request.VourcherId, request.PaymentMethod, request.cityCode, out responseStatus);
            if (responseStatus == EStatusCode.SUCCESS)
            {
                string keyRedis = "ORDER:" + accountId;
                await RedisGatewayCacheManager.Inst.DeleteDataFromCacheAsync(keyRedis).ConfigureAwait(false);
            }
            return Ok(new ResponseApiModel<OrderInfoObject>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = data });
        }
        [HttpPost]
        [Route("CreateOrderVer2")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> CreateOrderVer2(RequestCreateOrderModel request)
        {
            int responseStatus = -99;
            var clientInfo = new ClientRequestInfo(Request);
            string key = "SPAM:CREATE_ORDER" + clientInfo.TrueClientIp + "-" + clientInfo.DeviceId;
            if (RedisGatewayCacheManager.Inst.CheckExistKey(key))
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.TRANSACTION_SPAM, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.TRANSACTION_SPAM) });
            long accountId = await _tokenManager.GetAccountIdByAccessTokenAsync(Request);
            if (accountId <= 0)
                return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });
            await RedisGatewayCacheManager.Inst.SaveDataSecond(key, "1", 3).ConfigureAwait(false);
            //check barcode
            if (string.IsNullOrEmpty(request.Barcodes) || string.IsNullOrEmpty(request.Numbers))
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.ORDER_NOT_DATA, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.ORDER_NOT_DATA) });
            var dataBook = request.Barcodes.Split(",");
            var dataNumber = request.Numbers.Split(",");
            if (dataBook.Length != dataNumber.Length)
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.ORDER_NOT_DATA, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.ORDER_NOT_DATA) });
            try
            {
                for (int i = 0; i < dataNumber.Length; i++)
                {
                    var check = int.Parse(dataNumber[i]);
                }
            }
            catch
            {
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.ORDER_NOT_DATA, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.ORDER_NOT_DATA) });
            }
            var data = StoreOrderSqlInstance.Inst.CreateNewOrder_v2(accountId, request.CustomerId, request.Type, request.Description, request.Barcodes, request.Numbers, request.VourcherId,
                request.PaymentMethod, request.cityCode, out responseStatus);
            if (responseStatus == EStatusCode.SUCCESS)
            {
                string keyRedis = "ORDER:" + accountId;
                await RedisGatewayCacheManager.Inst.DeleteDataFromCacheAsync(keyRedis).ConfigureAwait(false);
            }
            data.VourcherId = request.VourcherId;
            return Ok(new ResponseApiModel<OrderInfoObject>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = data });
        }

        [HttpPost]
        [Route("CreateOrderNhanh")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> CreateOrderNhanh(RequestCreateOrderModel request)
        {
            int responseStatus = -99;
            var clientInfo = new ClientRequestInfo(Request);
            string key = "SPAM:CREATE_ORDER" + clientInfo.TrueClientIp + "-" + clientInfo.DeviceId;
            if (RedisGatewayCacheManager.Inst.CheckExistKey(key))
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.TRANSACTION_SPAM, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.TRANSACTION_SPAM) });
            long accountId = await _tokenManager.GetAccountIdByAccessTokenAsync(Request);
            if (accountId <= 0)
                return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });
            await RedisGatewayCacheManager.Inst.SaveDataSecond(key, "1", 3).ConfigureAwait(false);
            //check barcode
            if (string.IsNullOrEmpty(request.Barcodes) || string.IsNullOrEmpty(request.Numbers))
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.ORDER_NOT_DATA, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.ORDER_NOT_DATA) });
            var dataBook = request.Barcodes.Split(",");
            var dataNumber = request.Numbers.Split(",");
            if (dataBook.Length != dataNumber.Length)
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.ORDER_NOT_DATA, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.ORDER_NOT_DATA) });
            try
            {
                for (int i = 0; i < dataNumber.Length; i++)
                {
                    var check = int.Parse(dataNumber[i]);
                }
            }
            catch
            {
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.ORDER_NOT_DATA, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.ORDER_NOT_DATA) });
            }
            var data = StoreOrderSqlInstance.Inst.CreateNewOrder_v2(accountId, request.CustomerId, request.Type, request.Description, request.Barcodes, request.Numbers, request.VourcherId,
                request.PaymentMethod, request.cityCode, out responseStatus);
            if (responseStatus == EStatusCode.SUCCESS)
            {
                string keyRedis = "ORDER:" + accountId;
                await RedisGatewayCacheManager.Inst.DeleteDataFromCacheAsync(keyRedis).ConfigureAwait(false);
            }
            data.VourcherId = request.VourcherId;
            return Ok(new ResponseApiModel<OrderInfoObject>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = data });
        }

        [HttpGet]
        [Route("GetOrder")]
        [ResponseCache(Duration = 5)]
        public async Task<IActionResult> GetOrder()
        {
            long accountId = await _tokenManager.GetAccountIdByAccessTokenAsync(Request);
            if (accountId <= 0)
                return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });
            int responseStatus = EStatusCode.DATABASE_ERROR;
            string keyRedis = "ORDER:" + accountId;
            string jsonTag = await RedisGatewayCacheManager.Inst.GetDataFromCacheAsync(keyRedis);
            try
            {
                if (string.IsNullOrEmpty(jsonTag))
                {
                    var data = StoreOrderSqlInstance.Inst.GetOrderByAccountId(accountId, out responseStatus);
                    //update cache
                    if (responseStatus == EStatusCode.SUCCESS)
                    {
                        jsonTag = JsonConvert.SerializeObject(data);
                        await RedisGatewayCacheManager.Inst.SaveDataSecond(keyRedis, jsonTag, 300).ConfigureAwait(false);
                    }
                }
                else
                {
                    responseStatus = EStatusCode.SUCCESS;
                }
            }
            catch
            {

            }
            return Ok(new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = jsonTag });
        }

        [HttpPost]
        [Route("UpdateOrderEnd")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> UpdateOrderEnd(long OrderId)
        {
            int responseStatus = -99;
            long accountId = await _tokenManager.GetAccountIdByAccessTokenAsync(Request);
            if (accountId <= 0)
                return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });

            var data = StoreOrderSqlInstance.Inst.UserEndOrder(accountId, OrderId, 4, "User "+ accountId+" huy order", out responseStatus);
            if (responseStatus == 0)
            {
                string keyRedis = "ORDER:" + accountId;
                await RedisGatewayCacheManager.Inst.DeleteDataFromCacheAsync(keyRedis).ConfigureAwait(false);
            }
            return Ok(new ResponseApiModel<OrderInfoObject>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = data });
        }
        #endregion

        #region ORDER NHANH
        [HttpPost]
        [Route("CreateOrderNhanh2")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> CreateOrderNhanh2(RequestCreateOrderModel request)
        {
            int responseStatus = -99;
            var clientInfo = new ClientRequestInfo(Request);
            string key = "SPAM:CREATE_ORDER" + clientInfo.TrueClientIp + "-" + clientInfo.DeviceId;
            if (RedisGatewayCacheManager.Inst.CheckExistKey(key))
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.TRANSACTION_SPAM, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.TRANSACTION_SPAM) });
            long accountId = await _tokenManager.GetAccountIdByAccessTokenAsync(Request);
            if (accountId <= 0)
                return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });
            await RedisGatewayCacheManager.Inst.SaveDataSecond(key, "1", 3).ConfigureAwait(false);
            //check barcode
            if (string.IsNullOrEmpty(request.Barcodes) || string.IsNullOrEmpty(request.Numbers))
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.ORDER_NOT_DATA, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.ORDER_NOT_DATA) });
            var dataBook = request.Barcodes.Split(",");
            var dataNumber = request.Numbers.Split(",");
            if (dataBook.Length != dataNumber.Length)
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.ORDER_NOT_DATA, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.ORDER_NOT_DATA) });
            try
            {
                for (int i = 0; i < dataNumber.Length; i++)
                {
                    var check = int.Parse(dataNumber[i]);
                }
            }
            catch
            {
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.ORDER_NOT_DATA, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.ORDER_NOT_DATA) });
            }
            var data = StoreOrderSqlInstance.Inst.CreateNewOrder(accountId, request.CustomerId, request.Type, request.Description, request.Barcodes, request.Numbers, request.VourcherId, request.PaymentMethod, request.cityCode, out responseStatus);
            if (responseStatus == EStatusCode.SUCCESS)
            {
                string keyRedis = "ORDER:" + accountId;
                await RedisGatewayCacheManager.Inst.DeleteDataFromCacheAsync(keyRedis).ConfigureAwait(false);
            }
            return Ok(new ResponseApiModel<OrderInfoObject>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = data });
        }

        [HttpGet]
        [Route("GetOrderNhanh")]
        [ResponseCache(Duration = 5)]
        public async Task<IActionResult> GetOrderNhanh()
        {
            long accountId = await _tokenManager.GetAccountIdByAccessTokenAsync(Request);
            if (accountId <= 0)
                return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });
            int responseStatus = EStatusCode.DATABASE_ERROR;
            string keyRedis = "ORDER:" + accountId;
            string jsonTag = await RedisGatewayCacheManager.Inst.GetDataFromCacheAsync(keyRedis);
            try
            {
                if (string.IsNullOrEmpty(jsonTag))
                {
                    var data = StoreOrderSqlInstance.Inst.GetOrderByAccountId(accountId, out responseStatus);
                    //update cache
                    if (responseStatus == EStatusCode.SUCCESS)
                    {
                        jsonTag = JsonConvert.SerializeObject(data);
                        await RedisGatewayCacheManager.Inst.SaveDataSecond(keyRedis, jsonTag, 300).ConfigureAwait(false);
                    }
                }
                else
                {
                    responseStatus = EStatusCode.SUCCESS;
                }
            }
            catch
            {

            }
            return Ok(new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = jsonTag });
        }

        [HttpPost]
        [Route("UpdateOrderNhanhEnd")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> UpdateOrderNhanhEnd(long OrderId)
        {
            int responseStatus = -99;
            long accountId = await _tokenManager.GetAccountIdByAccessTokenAsync(Request);
            if (accountId <= 0)
                return Ok(new ResponseApiModel<string>() { Status = accountId, Messenger = UltilsHelper.GetMessageByErrorCode((int)accountId) });

            var data = StoreOrderSqlInstance.Inst.UserEndOrder(accountId, OrderId, 4, "User " + accountId + " huy order", out responseStatus);
            if (responseStatus == 0)
            {
                string keyRedis = "ORDER:" + accountId;
                await RedisGatewayCacheManager.Inst.DeleteDataFromCacheAsync(keyRedis).ConfigureAwait(false);
            }
            return Ok(new ResponseApiModel<OrderInfoObject>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = data });
        }
        #endregion
    }
}
