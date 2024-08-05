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
using ShareData.DB.Order;
using ShareData.DB.Users;
using ShareData.ErrorCode;
using ShareData.Request;
using ShareData.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using UtilsSystem.SocialNetwork;
using UtilsSystem.Utils;

namespace BookStoreCMS.Controllers
{
    [ApiVersion("1.0")]
    [Route("v1/OrderCms")]
    [ApiController]
    public class OrderCMSController : ControllerBase
    {
        private ILoggerManager _logger;
        private IEmailSender _emailSender;
        private ITokenManager _tokenManager;
        public OrderCMSController(ILoggerManager logger, IEmailSender emailSender, ITokenManager tokenManager)
        {
            _logger = logger;
            _emailSender = emailSender;
            _tokenManager = tokenManager;
        }
        private string token = string.Empty;
        [HttpPost]
        [Route("CreateOrder")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> CreateOrder(RequestCreateOrderCmsModel request)
        {
            int responseStatus = -99;
            var clientInfo = new ClientRequestInfo(Request);
            string key = "SPAM:CREATE_ORDER" + clientInfo.TrueClientIp + "-" + clientInfo.DeviceId;
            if (RedisGatewayCacheManager.Inst.CheckExistKey(key))
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.TRANSACTION_SPAM, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.TRANSACTION_SPAM) });
            await RedisGatewayCacheManager.Inst.SaveDataSecond(key, "1", 5).ConfigureAwait(false);

            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });

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
            var data = StoreOrderSqlInstance.Inst.CreateNewOrderCMS(request.AccountId, request.CustomerName, request.CustomerMobile, request.CustomerEmail, request.CustomerAddress, request.Type, request.Description, request.Barcodes, request.Numbers, request.PaymentMethod, request.ShipMoney, request.TotalDiscountMoney, request.VourcherMoney, out responseStatus);

            return Ok(new ResponseApiModel<OrderInfoObject>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = data });
        }

        [HttpGet]
        [Route("GetserviceIdGhn")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> GetserviceIdGhn(int todistrict)
        {
            string dataGhn = "";
            try
            {
                dataGhn = await GhnInstance.Inst.SendGetserviceIdGhn(NO_SQL_CONFIG.from_district_code, todistrict);
                ResponseGhnModel<ServiceId[]> modelResponseGghn = JsonConvert.DeserializeObject<ResponseGhnModel<ServiceId[]>>(dataGhn);
                if (modelResponseGghn.code == 200)
                {
                    return Ok(new ResponseApiModel<ServiceId[]>() { Status = 0, Messenger = UltilsHelper.GetMessageByErrorCode(0), DataResponse = modelResponseGghn.data });
                }
                else
                {
                    return Ok(new ResponseApiModel<string>() { Status = modelResponseGghn.code, Messenger = modelResponseGghn.message, DataResponse = dataGhn });
                }
            }
            catch (Exception ex)
            {
                await _logger.LogError("GetserviceIdGhn", ex.ToString()+" - "+dataGhn).ConfigureAwait(false);
                return Ok(new ResponseApiModel<OrderInfoObject>() { Status = EStatusCode.DATA_INVAILD, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.DATA_INVAILD) });
            }
        }

        [HttpPost]
        [Route("ChangeOrderProcess")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> ChangeOrderProcess(RequestProcessOrderModel requestchange)
        {
            int responseStatus = -99;
            var clientInfo = new ClientRequestInfo(Request);
            string key = "SPAM:PROCESS_ORDER" + clientInfo.TrueClientIp + "-" + clientInfo.DeviceId;
            if (RedisGatewayCacheManager.Inst.CheckExistKey(key))
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.TRANSACTION_SPAM, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.TRANSACTION_SPAM) });
            await RedisGatewayCacheManager.Inst.SaveDataSecond(key, "1", 5).ConfigureAwait(false);

            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });
            //get info Order by id
            var listModel = StoreOrderSqlInstance.Inst.GetOrderById(requestchange.OrderId, out responseStatus);
            var modelOrder = listModel.FirstOrDefault();
            if (modelOrder == null) {
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.ID_NOT_EXITS, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.ID_NOT_EXITS) });
            }
            if(string.IsNullOrEmpty(modelOrder.CustomerAddress))
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.ADRESS_NOT_EXITS, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.ADRESS_NOT_EXITS) });
            //list item
            ItemGhnModel[] listItem = null;
            //parse model adress
            AdressModel modelAdress = JsonConvert.DeserializeObject<AdressModel>(modelOrder.CustomerAddress);
            //send GHN
            var dataGhn = await GhnInstance.Inst.SendCreateOrderGhn(modelOrder.Description, modelOrder.CustomerName, modelOrder.CustomerMobile, modelAdress.AdrDetail, modelAdress.AdrWard, modelAdress.AdrdDistrict, modelAdress.AdrCity, modelOrder.TotalMoney, 
                requestchange.PrivateDescription, requestchange.weight, requestchange.length, requestchange.width, requestchange.height, listItem, requestchange.service_id, requestchange.service_type_id);

            ResponseGhnModel<ResponseGhnData> modelResponseGghn = null;
            await _logger.LogError("Account-ChangeOrderProcess GHN{}", dataGhn + "  " + JsonConvert.SerializeObject(modelOrder) + "  "+JsonConvert.SerializeObject(requestchange)).ConfigureAwait(false);
            try
            {
                modelResponseGghn = JsonConvert.DeserializeObject<ResponseGhnModel<ResponseGhnData>>(dataGhn);
                if (modelResponseGghn.code == 200)
                {
                    long totalFee = 0;
                    long.TryParse(modelResponseGghn.data.total_fee, out totalFee);
                    ////send mail
                    //var message = new Message(new string[] { modelOrder.CustomerEmail }, "Xác nhận đơn hàng Gamma Books",
                    //  "Chào bạn " + modelOrder.CustomerEmail.Split("@")[0] + "!" + Environment.NewLine +
                    //  "Cảm ơn bạn đã đặt hàng trên ứng dụng Gamma Books." + Environment.NewLine +
                    //  "Dưới đây là thông tin chi tiết về đơn hàng của bạn:" + Environment.NewLine +

                    //  "Đơn hàng [" + modelResponseGghn.data.order_code + "] đã được tạo và sẽ được chuyển tới bạn trong thời gian sớm nhất." + Environment.NewLine +
                    //  "Bạn có thể theo dõi trạng thái đơn hàng tại địa chỉ: https://ghn.vn" + Environment.NewLine +
                    //  "Nếu bạn cần hỗ trợ, vui lòng gọi theo số hotline: 0934466060, hoặc liên hệ fanpage của Gamma để được giải đáp mọi thắc mắc."
                    //  );
                    //await _emailSender.SendEmailAsync(message);
                    //save to db
                    var data = StoreOrderSqlInstance.Inst.ChangeOrderProcess(requestchange.OrderId, totalFee, modelResponseGghn.data.order_code, requestchange.AllowTest, out responseStatus);
                    return Ok(new ResponseApiModel<OrderInfoObject>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = data });
                }
                else
                {
                    return Ok(new ResponseApiModel<OrderInfoObject>() { Status = modelResponseGghn.code, Messenger = modelResponseGghn.message });
                }
            }
            catch (Exception ex)
            {
                await _logger.LogError("Account-ChangeOrderProcess GHN{}", ex.ToString() + "  " + JsonConvert.SerializeObject(modelResponseGghn)).ConfigureAwait(false);
                return Ok(new ResponseApiModel<OrderInfoObject>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) });
            }
        }

        [HttpPost]
        [Route("UpdateOrderEnd")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> UpdateOrderEnd(RequestEndOrderModel requestchange)
        {
            int responseStatus = -99;
            var clientInfo = new ClientRequestInfo(Request);
            string key = "SPAM:END_ORDER" + clientInfo.TrueClientIp + "-" + clientInfo.DeviceId;
            if (RedisGatewayCacheManager.Inst.CheckExistKey(key))
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.TRANSACTION_SPAM, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.TRANSACTION_SPAM) });
            await RedisGatewayCacheManager.Inst.SaveDataSecond(key, "1", 5).ConfigureAwait(false);

            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });

            var data = StoreOrderSqlInstance.Inst.EndOrder(requestchange.OrderId, requestchange.Status, requestchange.PrivateDescription, out responseStatus);

            return Ok(new ResponseApiModel<OrderInfoObject>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = data });
        }
        [HttpPost]
        [Route("GetOrderByTime")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> GetOrderByTime(RequestGetByTime requestchange)
        {
            int responseStatus = -99;
            var clientInfo = new ClientRequestInfo(Request);
            string key = "SPAM:GET_LIST_ORDER" + clientInfo.TrueClientIp + "-" + clientInfo.DeviceId;
            if (RedisGatewayCacheManager.Inst.CheckExistKey(key))
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.TRANSACTION_SPAM, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.TRANSACTION_SPAM) });
            await RedisGatewayCacheManager.Inst.SaveDataSecond(key, "1", 5).ConfigureAwait(false);

            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });

            var data = StoreOrderSqlInstance.Inst.GetOrderByTime(requestchange.StartTime, requestchange.EndTime, out responseStatus);

            return Ok(new ResponseApiModel<List<OrderInfoObject>>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = data });
        }


        #region INFO SHIP
        [HttpPost]
        [Route("CreateOrderInfo")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> CreateOrderInfo(RequestCreateOrderInfoModelCMS request)
        {
            int responseStatus = -99;
            var clientInfo = new ClientRequestInfo(Request);
            string key = "SPAM:CREATE_ORDER_INFO" + clientInfo.TrueClientIp + "-" + clientInfo.DeviceId;
            if (RedisGatewayCacheManager.Inst.CheckExistKey(key))
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.TRANSACTION_SPAM, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.TRANSACTION_SPAM) });

            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });

            await RedisGatewayCacheManager.Inst.SaveDataSecond(key, "1", 3).ConfigureAwait(false);

            var data = StoreOrderSqlInstance.Inst.CreateCustomerInfo(request.AccountId, request.CustomerName, request.CustomerMobile, request.CustomerEmail, request.CustomerAddress, request.Defaut, out responseStatus);
            //update cache
            if (responseStatus == EStatusCode.SUCCESS)
            {
                string keyRedis = "CUSTOMER:" + request.AccountId;
                await RedisGatewayCacheManager.Inst.SaveDataSecond(keyRedis, JsonConvert.SerializeObject(data), 300).ConfigureAwait(false);
            }
            return Ok(new ResponseApiModel<List<CustomerInfoModel>>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = data });
        }

        [HttpGet]
        [Route("GetOrderInfo")]
        [ResponseCache(Duration = 5)]
        public async Task<IActionResult> GetOrderInfo(long AccountId)
        {
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });

            int responseStatus = EStatusCode.DATABASE_ERROR;
            string keyRedis = "CUSTOMER:" + AccountId;
            string jsonTag = await RedisGatewayCacheManager.Inst.GetDataFromCacheAsync(keyRedis);
            try
            {
                if (string.IsNullOrEmpty(jsonTag))
                {
                    var data = StoreOrderSqlInstance.Inst.GetCustomerInfo(AccountId, out responseStatus);
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
        #endregion

        [HttpPost]
        [Route("TestSendMail")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> TestSendMail(string CustomerEmail)
        {
            try
            {
                //send mail
                var message = new Message(new string[] { CustomerEmail }, "Xác nhận đơn hàng Gamma Books",
                    "Chào bạn " + CustomerEmail.Split("@")[0] + "!" + Environment.NewLine +
                    "Cảm ơn bạn đã đặt hàng trên ứng dụng Gamma Books." + Environment.NewLine +
                    "Dưới đây là thông tin chi tiết về đơn hàng của bạn:" + Environment.NewLine +

                    "Đơn hàng [" + "123456" + "] đã được tạo và sẽ được chuyển tới bạn trong thời gian sớm nhất." + Environment.NewLine +
                    "Bạn có thể theo dõi trạng thái đơn hàng tại địa chỉ: https://ghn.vn" + Environment.NewLine +
                    "Nếu bạn cần hỗ trợ, vui lòng gọi theo số hotline: 0934466060, hoặc liên hệ fanpage của Gamma để được giải đáp mọi thắc mắc."
                    );
                await _emailSender.SendEmailAsync(message);
                //save to db
                return Ok(new ResponseApiModel<OrderInfoObject>() { Status = 0, Messenger = UltilsHelper.GetMessageByErrorCode(0), DataResponse = null });
            } catch (Exception ex)
            {
                await _logger.LogError("TestSendMail", ex.ToString() ).ConfigureAwait(false);
                return Ok();
            }

        }
    }

}
