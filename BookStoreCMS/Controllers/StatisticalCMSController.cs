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
        [ResponseCache(Duration = 60)]
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
        [Route("GetCountRegis")]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetCountRegis(RequestGetRegisCmsModel requestRegis)
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
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetCountMember(RequestGetMemberCmsModel requestMember)
        {
            int responseStatus = -99;
            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });

            var data = StoreMemberSqlInstance.Inst.GetVipCountByTime(requestMember.Vip, requestMember.StartTime, requestMember.EndTime, out responseStatus);

            return Ok(new ResponseApiModel<long>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = data });
        }
    }
}
