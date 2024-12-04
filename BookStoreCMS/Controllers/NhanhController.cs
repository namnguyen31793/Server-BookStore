using BookStoreCMS.Instance;
using BookStoreCMS.Interfaces;
using BookStoreCMS.Utils;
using DAO.DAOImp;
using LoggerService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.X509;
using RedisSystem;
using ShareData.API;
using ShareData.DataEnum;
using ShareData.DB.Order;
using ShareData.DB.Vourcher;
using ShareData.ErrorCode;
using ShareData.Request;
using ShareData.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using UtilsSystem.Utils;
using static MongoDB.Driver.WriteConcern;

namespace BookStoreCMS.Controllers
{
    [ApiVersion("1.0")]
    [Route("v1/NhanhHandle")]
    [ApiController]
    public class NhanhController : ControllerBase
    {
        private ILoggerManager _logger;
        private IEmailSender _emailSender;
        private ITokenManager _tokenManager;
        public NhanhController(ILoggerManager logger, IEmailSender emailSender, ITokenManager tokenManager)
        {
            _logger = logger;
            _emailSender = emailSender;
            _tokenManager = tokenManager;
        }
        private string token = string.Empty;

        [HttpPost] [HttpGet]
        [Route("RedirectAccessCode")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async System.Threading.Tasks.Task<IActionResult> RedirectAccessCode(string accessCode)
        {
            string directoryPath = @"C:\WebServices\LogicCms";
            string fileName = "accessCodeNhanh.txt";
            string filePath = Path.Combine(directoryPath, fileName);
            try
            {
                // Kiểm tra và tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Ghi nội dung vào tệp
                System.IO.File.AppendAllText(filePath, accessCode);

            }
            catch (Exception ex)
            {
            }
            await _logger.LogError("GetserviceIdNhanh", accessCode).ConfigureAwait(false);
            return Ok();
        }

        [HttpPost]
        [HttpGet]
        [Route("Webhooks")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async System.Threading.Tasks.Task<IActionResult> Webhooks(string accessCode)
        {

            await _logger.LogError("GetserviceIdNhanh", accessCode).ConfigureAwait(false);
            return Ok();
        }

        [HttpGet]
        [Route("NhanhLocation")]
        public async System.Threading.Tasks.Task<IActionResult> NhanhLocation(string Type, int ParentId = 0)
        {
            string dataNhanh = "";
            string key = "NhanhLocation:CITY";
            if (Type.Equals("CITY"))
            {
                if (RedisGatewayCacheManager.Inst.CheckExistKey(key))
                    dataNhanh = RedisGatewayCacheManager.Inst.GetDataFromCache(key);
            }
            else {
                key = "NhanhLocation:"+ Type +"-"+ ParentId;
                if (RedisGatewayCacheManager.Inst.CheckExistKey(key))
                    dataNhanh = RedisGatewayCacheManager.Inst.GetDataFromCache(key);
            }
            if (string.IsNullOrEmpty(dataNhanh)) {
                var obj = new { type = Type, parentId = ParentId };
                try
                {
                    dataNhanh = await NhanhInstance.Inst.SendServiceIdNhanh("/api/shipping/location", JsonConvert.SerializeObject(obj));

                }
                catch (Exception ex)
                {
                    await _logger.LogError("Account-NhanhLocation{}", ex.ToString()).ConfigureAwait(false);
                }
                await RedisGatewayCacheManager.Inst.SaveDataAsync(key, dataNhanh, 60).ConfigureAwait(false);
            }

            return Ok(new ResponseApiModel<string>() { Status = 0, Messenger = UltilsHelper.GetMessageByErrorCode(0), DataResponse = dataNhanh });
        }

        //lay id kho cua minh
        [HttpPost]
        [Route("GetFee")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async System.Threading.Tasks.Task<IActionResult> GetFee(RequestGetFeeNhanhModel model)
        {
            var obj = new { fromCityName = model.fromCityName, fromDistrictName = model.fromDistrictName, toCityName = model.toCityName, toDistrictName = model.toDistrictName
                , codMoney = model.codMoney, shippingWeight = model.shippingWeight, carrierIds = model.carrierIds, length = model.length, width = model.width, height = model.height };

            string dataNhanh = await NhanhInstance.Inst.SendServiceIdNhanh("/api/shipping/fee", JsonConvert.SerializeObject(obj));

            await _logger.LogError("Account-GetFee{}", dataNhanh + "  " + JsonConvert.SerializeObject(obj)).ConfigureAwait(false);
            return Ok(new ResponseApiModel<string>() { Status = 0, Messenger = UltilsHelper.GetMessageByErrorCode(0), DataResponse = dataNhanh });
        }
        //id hang van chuyen
        [HttpGet]
        [Route("GetCarrierId")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async System.Threading.Tasks.Task<IActionResult> GetCarrierId(int depotId = 0)
        {
            string dataNhanh = "";
            string key = "NhanhCarrierId";
            if (RedisGatewayCacheManager.Inst.CheckExistKey(key))
                dataNhanh = RedisGatewayCacheManager.Inst.GetDataFromCache(key);

            if (string.IsNullOrEmpty(dataNhanh))
            {
                var obj = new { depotId = depotId };

                dataNhanh = await NhanhInstance.Inst.SendServiceIdNhanh("/api/store/depot", JsonConvert.SerializeObject(obj));

                await _logger.LogError("Account-GetCarrierId{}", dataNhanh + "  " + JsonConvert.SerializeObject(obj)).ConfigureAwait(false);
                await RedisGatewayCacheManager.Inst.SaveDataAsync(key, dataNhanh, 60).ConfigureAwait(false);
            }

            return Ok(new ResponseApiModel<string>() { Status = 0, Messenger = UltilsHelper.GetMessageByErrorCode(0), DataResponse = dataNhanh });
        }

        [HttpPost]
        [Route("ChangeOrderProcessNhanh")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async System.Threading.Tasks.Task<IActionResult> ChangeOrderProcessNhanh(RequestProcessOrderNhanhModel requestchange)
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
            if (modelOrder == null)
            {
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.ID_NOT_EXITS, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.ID_NOT_EXITS) });
            }
            if (string.IsNullOrEmpty(modelOrder.CustomerAddress))
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.ADRESS_NOT_EXITS, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.ADRESS_NOT_EXITS) });
            //list item
            AdressModel modelAdress = JsonConvert.DeserializeObject<AdressModel>(modelOrder.CustomerAddress);
            string BarcodeStrings = modelOrder.TempBarcodes;
            string BarcodeNumberStrings = modelOrder.TempNumbers;
            var listBarcode = BarcodeStrings.Split(',');
            var listNumber = BarcodeNumberStrings.Split(',');

            bool useVourcher = false;
            string nameVourcher = "";
            if (modelOrder.VourcherId > 0)
            {
                useVourcher = await CheckHaveUseVourcherAsync(modelOrder.VourcherId, BarcodeStrings, modelOrder.TotalBaseMoney);
                var data = await GetInfoVourcherAsync(modelOrder.VourcherId);
                nameVourcher = data.VourcherName;
            }

            List<ProductModel> listProductModel = new List<ProductModel>();
            for (int i = 0; i < listBarcode.Count(); i++) {
                var book = StoreBookSqlInstance.Inst.GetBookCmsByBarcode(listBarcode[i], out responseStatus);
                var price = (int)book.AmountSale;
                if(modelOrder.VourcherId > 0 && useVourcher)
                    price = await CheckDiscountVourcherAsync(modelOrder.VourcherId, price);
                
                ProductModel productList = new ProductModel() {
                    id = listBarcode[i],
                    idNhanh = 0,
                    quantity = int.Parse(listNumber[i]),
                    name = book.BookName,
                    code = book.Barcode,
                    imei = "",
                    type = "Product",
                    price = price,
                    weight = 1,
                    importPrice = 0,
                    description = ""//book.ContentBook
                };
                listProductModel.Add(productList);
            }
            //data send
            RequestAddNhanhModel dataSendNhanh = new RequestAddNhanhModel()
            {
                id = requestchange.OrderId.ToString(),
                depotId = (int)requestchange.DepotId,
                type = requestchange.Type,
                customerName = modelOrder.CustomerName,
                customerMobile = modelOrder.CustomerMobile,
                customerEmail = modelOrder.CustomerEmail,
                customerAddress = modelAdress.AdrDetail +", "+ modelAdress.AdrdDistrict + ", " + modelAdress.AdrCity,
                customerCityName = modelAdress.AdrCity,
                customerDistrictName = modelAdress.AdrdDistrict,
                customerWardLocationName = modelAdress.AdrWard,
                moneyDiscount = 0,
                moneyTransfer = 0,
                moneyTransferAccountId = 0,
                moneyDeposit = 0,
                moneyDepositAccountId = 0,
                paymentMethod = "COD",
                paymentCode = "",
                paymentGateway = "",
                carrierId = requestchange.carrierId,
                carrierServiceId = requestchange.carrierServiceId,
                customerShipFee = (int)modelOrder.ShipMoney,
                deliveryDate = DateTime.Now.ToString("yyyy-MM-dd"),
                status = "New",
                description = modelOrder.Description,
                privateDescription = requestchange.PrivateDescription,
                productList = listProductModel.ToArray(),
                trafficSource = "app",
                couponCode = nameVourcher,
                allowTest = requestchange.AllowTest,
                saleId = 0,
                autoSend = 0,
                sendCarrierType = 1,
                carrierAccountId = 0,
                carrierShopId = 0,
                carrierServiceCode = "",
                utmCampaign = NO_SQL_CONFIG.NHANH_utmCampaign,
                utmSource = NO_SQL_CONFIG.NHANH_utmSource,
                utmMedium = NO_SQL_CONFIG.NHANH_utmMedium,
                usedPoints = 0
            };
            //send GHN
            //var obj = new { id = requestchange.OrderId, depotId = requestchange.DepotId, type = requestchange.Type, customerName = modelOrder.CustomerName, customerMobile = modelOrder.CustomerMobile, customerEmail = modelOrder.CustomerEmail
            //, customerAddress = modelOrder.CustomerAddress, customerCityName = modelAdress.AdrCity, customerDistrictName = modelAdress.AdrdDistrict, customerWardLocationName = modelAdress.AdrWard
            //, moneyDiscount = 0, moneyTransfer = 0, moneyTransferAccountId = 0, moneyDeposit = 0, moneyDepositAccountId = 0
            //, paymentMethod = "COD", paymentCode = "", paymentGateway = "", carrierId = requestchange.carrierId, carrierServiceId = requestchange.carrierServiceId, customerShipFee = modelOrder.ShipMoney
            //, deliveryDate = DateTime.Now.ToString("yyyy-mm-dd"), status = "New", description = modelOrder.Description, privateDescription = requestchange.PrivateDescription
            //, trafficSource = "app", productList = new { }, couponCode = modelOrder.VourcherId, allowTest = requestchange.AllowTest, saleId = 0, autoSend = 0, sendCarrierType = 1
            //, carrierAccountId = 0, carrierShopId = 0, carrierServiceCode = "", utmCampaign = NO_SQL_CONFIG.NHANH_utmCampaign, utmSource = NO_SQL_CONFIG.NHANH_utmSource, utmMedium = NO_SQL_CONFIG.NHANH_utmMedium
            //, usedPoints = 0
            //};
            var dataNhanh = await NhanhInstance.Inst.SendServiceIdNhanh("/api/order/add", JsonConvert.SerializeObject(dataSendNhanh));

            try
            {
                var modelResponseGghn = JsonConvert.DeserializeObject<ResponseGhnModel<ResponseNhanhData>>(dataNhanh);
                if (modelResponseGghn.code == 1)
                {
                    long totalFee = modelResponseGghn.data.shipFee + modelResponseGghn.data.declaredFee ;
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
                    var data = StoreOrderSqlInstance.Inst.ChangeOrderProcess(requestchange.OrderId, totalFee, "ID Nhanh: "+ modelResponseGghn.data.orderId, requestchange.AllowTest, out responseStatus);
                    return Ok(new ResponseApiModel<OrderInfoObject>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = data });
                }
                else
                {
                    string dataString = JsonConvert.SerializeObject(dataSendNhanh);
                    await _logger.LogError("Account-ChangeOrderProcessNhanh{}", dataNhanh + "  " + dataString).ConfigureAwait(false);
                    return Ok(new ResponseApiModel<OrderInfoObject>() { Status = EStatusCode.SYSTEM_EXCEPTION, Messenger = dataNhanh });
                }
            }
            catch (Exception ex)
            {
                await _logger.LogError("Account-ChangeOrderProcessNhanh{}", ex.ToString()).ConfigureAwait(false);
                return Ok(new ResponseApiModel<OrderInfoObject>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) });
            }
        }

        private async Task<VourcherCheck> GetInfoVourcherAsync(int VourcherId) {
            int VourcherType = 0;
            string VourcherName = "";
            string VourcherReward = "";
            string Target = "";
            string keyRedis = "AllVourcher:" + VourcherId;
            string jsonString = await RedisGatewayCacheManager.Inst.GetDataFromCacheAsync(keyRedis);
            if (string.IsNullOrEmpty(jsonString))
            {
                var res = StoreVourcherSqlInstance.Inst.UserGetVourcher_ById(VourcherId, out VourcherType, out VourcherName, out VourcherReward, out Target);
                if (res >= 0)
                    jsonString = VourcherType + "-" + VourcherName + "-" + VourcherReward + "-" + Target;
                await RedisGatewayCacheManager.Inst.SaveDataAsync(keyRedis, jsonString, 2);
            }
            else
            {
                var listData = jsonString.Split("-");
                VourcherType = int.Parse(listData[0]);
                VourcherName = listData[1];
                VourcherReward = listData[2];
                Target = listData[3];
            }
            return new VourcherCheck() { VourcherType = VourcherType, VourcherName = VourcherName, VourcherReward = VourcherReward, Target = Target };
        }
        private async Task<bool> CheckHaveUseVourcherAsync(int VourcherId, string barCode, long totalMoney)
        {
            bool haveuse = false;
            if (VourcherId >= 1 && VourcherId < 5)
            {
                return true;
            }
            var data = await GetInfoVourcherAsync(VourcherId);
            if (data != null) {
                if (string.IsNullOrEmpty(data.Target))
                    haveuse = true;
                else { 
                    var listTarget = data.Target.Split(',');
                    var listBook = barCode.Split(',');
                    var commonItems = listBook.Intersect(listTarget).ToList();
                    if (commonItems.Any())
                        haveuse = true;
                }
                if (VourcherId >= 5 && ( data.VourcherType == 1 || data.VourcherType == 2)) { 
                    var minMoney = Int32.Parse(data.VourcherReward.Split(',')[1]);
                    if (totalMoney < minMoney)
                        haveuse = false;
                }
            }
            return haveuse;
        }
        private async Task<int> CheckDiscountVourcherAsync(int VourcherId, int baseValue) {
            int price = baseValue;
            var data = await GetInfoVourcherAsync(VourcherId);
            if (data == null)  {
                return price;
            }
            if (VourcherId >= 1 && VourcherId < 5)
            {
                var saleDiscount = Int32.Parse(data.VourcherReward);
                price -= (int)(baseValue * saleDiscount / 100);
            }
            else
            {
                if (data.VourcherType == 1) {    //đạt min sẽ trừ theo %
                    var listData = data.VourcherReward.Split(',');
                    int saleDiscount = Int32.Parse(listData[2]);

                    price -= (int)(baseValue * saleDiscount / 100);
                }else if (data.VourcherType == 2)   //đạt min sẽ trừ tiền
                {
                    var listData = data.VourcherReward.Split(',');
                    int saleDiscount = Int32.Parse(listData[2]);

                    price -= (int)saleDiscount;
                }
            }
            return price;
        }
    }
}
