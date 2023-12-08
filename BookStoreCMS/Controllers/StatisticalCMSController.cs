using BookStoreCMS.Instance;
using BookStoreCMS.Interfaces;
using BookStoreCMS.Utils;
using DAO.DAOImp;
using LoggerService;
using LoggerService.Report;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RedisSystem;
using ShareData.API;
using ShareData.DataEnum;
using ShareData.DB.Order;
using ShareData.DB.Statstical;
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
    [Route("v1/StatisticalCMS")]
    [ApiController]
    public class StatisticalCMSController : ControllerBase
    {
        private ILoggerManager _logger;
        private IEmailSender _emailSender;
        private ITokenManager _tokenManager;
        public StatisticalCMSController(ILoggerManager logger, IEmailSender emailSender, ITokenManager tokenManager)
        {
            _logger = logger;
            _emailSender = emailSender;
            _tokenManager = tokenManager;
        }
        private string token = string.Empty;

        [HttpGet]
        [Route("GetAgeAvg")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> GetAgeAvg()
        {
            int responseStatus = -99;
            var clientInfo = new ClientRequestInfo(Request);
            string key = "SPAM:GetAgeAvg" + clientInfo.TrueClientIp + "-" + clientInfo.DeviceId;
            if (RedisGatewayCacheManager.Inst.CheckExistKey(key))
                return Ok(new ResponseApiModel<string>() { Status = EStatusCode.TRANSACTION_SPAM, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.TRANSACTION_SPAM) });
            await RedisGatewayCacheManager.Inst.SaveDataSecond(key, "1", 15).ConfigureAwait(false);

            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });

            var data = StoreUsersSqlInstance.Inst.GetAgeAvg(out responseStatus);

            return Ok(new ResponseApiModel<AgeAvgModel>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = data });
        }

        [HttpPost]
        [Route("GetCountRegisType")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> GetCountRegisType(RequestGetRegisCmsModel requestRegis)
        {
            int responseStatus = -99;
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });

            var data = StoreUsersSqlInstance.Inst.GetRegisByTime(requestRegis.Type, requestRegis.StartTime, requestRegis.EndTime, out responseStatus);

            return Ok(new ResponseApiModel<long>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = data });
        }

        [HttpPost]
        [Route("GetCountMember")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> GetCountMember(RequestGetMemberCmsModel requestMember)
        {
            int responseStatus = -99;
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });

            var data = StoreMemberSqlInstance.Inst.GetVipCountByTime(requestMember.Vip, requestMember.StartTime, requestMember.EndTime, out responseStatus);

            return Ok(new ResponseApiModel<long>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = data });
        }

        [HttpPost]
        [Route("GetCountRegis")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> GetCountRegis(RequestGetCountRegis request)
        {
            int responseStatus = -99;
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });

            var data = StoreUsersSqlInstance.Inst.GetCountRegis(request.Os, request.StartTime, request.EndTime, out responseStatus);

            return Ok(new ResponseApiModel<long>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = data });
        }

        [HttpPost]
        [Route("GetActionUser")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> GetActionUser(RequestGetAction request)
        {
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });

            var data = TrackingLogSystemInstance.Inst.Get_ActionUser(request.Action, request.StartTime, request.EndTime);

            return Ok(new ResponseApiModel<string>() { Status = 0, Messenger = "", DataResponse = JsonConvert.SerializeObject(data) });
        }

        [HttpPost]
        [Route("GetActionUserExtend")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> GetActionUserExtend(RequestGetActionExtend request)
        {
            //int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            //if (checkRole < 0)
            //    return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });

            var data = TrackingLogSystemInstance.Inst.Get_ActionUser(request.Action, request.Extension, request.StartTime, request.EndTime);

            return Ok(new ResponseApiModel<string>() { Status = 0, Messenger = "", DataResponse = JsonConvert.SerializeObject(data) });
        }

        [HttpPost]
        [Route("GetActionHome")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> GetActionHome(RequestGetAction request)
        {
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });

            var data = TrackingLogSystemInstance.Inst.Get_ActionHome(request.Action, request.StartTime, request.EndTime);

            return Ok(new ResponseApiModel<string>() { Status = 0, Messenger = "", DataResponse = JsonConvert.SerializeObject(data) });
        }

        [HttpPost]
        [Route("GetActionHomeExtend")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> GetActionHomeExtend(RequestGetActionExtend request)
        {
            //int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            //if (checkRole < 0)
            //    return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });

            var data = TrackingLogSystemInstance.Inst.Get_ActionHome(request.Action, request.Extension, request.StartTime, request.EndTime);

            return Ok(new ResponseApiModel<string>() { Status = 0, Messenger = "", DataResponse = JsonConvert.SerializeObject(data) });
        }

        [HttpPost]
        [Route("GetActionFindBook")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> GetActionFindBook(RequestGetAction request)
        {
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });

            var data = TrackingLogSystemInstance.Inst.Get_ActionFindBook(request.Action, request.StartTime, request.EndTime);

            return Ok(new ResponseApiModel<string>() { Status = 0, Messenger = "", DataResponse = JsonConvert.SerializeObject(data) });
        }

        [HttpGet]
        [Route("GetCcu")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> GetCcu()
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            string keyRedis = "Token";
            long ccu = await RedisGatewayCacheManager.Inst.CountItemByString(keyRedis);
            responseStatus = 0;
            response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = ccu.ToString() };
            return Ok(response);
        }

        [HttpPost]
        [Route("GetCcuByTime")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> GetCcuByTime(RequestGetByTime model)
        {

            var data = await Tracking_Online_Manager.Inst.GetListCcuByTime(model.StartTime, model.EndTime);

            var response = new ResponseApiModel<string>() { Status = 0, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SUCCESS), DataResponse = JsonConvert.SerializeObject(data) };
            return Ok(response);
        }

        [HttpGet]
        [Route("GetTopBookLike")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> GetTopBookLike()
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            var data = StoreBookSqlInstance.Inst.GetTopLike(out responseStatus);
            response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = JsonConvert.SerializeObject(data) };
            return Ok(response);
        }

        [HttpPost]
        [Route("GetTopBarcodeBuy")]
        public async Task<IActionResult> GetTopBarcodeBuy(RequestGetByTime model)
        {
            try
            {
                var data = TrackingLogSystemInstance.Inst.GetTopBuyBarcode(model.StartTime, model.EndTime);

                var response = new ResponseApiModel<string>() { Status = 0, Messenger = UltilsHelper.GetMessageByErrorCode(0), DataResponse = JsonConvert.SerializeObject(data) };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseApiModel<string>() { Status = 0, Messenger = ex.ToString() };
                return Ok(response);
            }
        }

        [HttpPost]
        [Route("GetTopBarcodeSearch")]
        public async Task<IActionResult> GetTopBarcodeSearch(RequestGetByTime model)
        {
            try
            {
                var data = TrackingLogSystemInstance.Inst.GetTopBookSearch();

                var response = new ResponseApiModel<string>() { Status = 0, Messenger = UltilsHelper.GetMessageByErrorCode(0), DataResponse = JsonConvert.SerializeObject(data) };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseApiModel<string>() { Status = 0, Messenger = ex.ToString() };
                return Ok(response);
            }
        }
    }
}
