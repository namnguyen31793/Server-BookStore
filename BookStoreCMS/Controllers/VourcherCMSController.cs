using BookStoreCMS.Instance;
using BookStoreCMS.Interfaces;
using BookStoreCMS.Utils;
using DAO.DAOImp;
using LoggerService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RedisSystem;
using ShareData.API;
using ShareData.DataEnum;
using ShareData.DB.Vourcher;
using ShareData.ErrorCode;
using ShareData.Request;
using System.Threading.Tasks;
using UtilsSystem.Utils;

namespace BookStoreCMS.Controllers
{
    [ApiVersion("1.0")]
    [Route("v1/VourcherCMS")]
    [ApiController]
    public class VourcherCMSController : ControllerBase
    {
        private ILoggerManager _logger;
        private IEmailSender _emailSender;
        private ITokenManager _tokenManager;
        public VourcherCMSController(ILoggerManager logger, IEmailSender emailSender, ITokenManager tokenManager)
        {
            _logger = logger;
            _emailSender = emailSender;
            _tokenManager = tokenManager;
        }
        private string token = string.Empty;

        [HttpPost]
        [Route("CreateVourcher")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> CreateVourcher(RequestCreateVourcherCmsModel request)
        {
            int responseStatus = -99;

            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });

            var data = StoreVourcherSqlInstance.Inst.AddVourcher(request.VourcherName, request.VourcherType, request.CountUse, request.VourcherReward, request.VourcherDescription, request.thumbnail, request.Targets, request.StartTime, request.EndTime, request.Status, out responseStatus);

            return Ok(new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = JsonConvert.SerializeObject(data) });
        }

        [HttpPost]
        [Route("UpdateVourcher")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> UpdateVourcher(RequestCreateVourcherCmsModel request)
        {
            int responseStatus = -99;

            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });

            var data = StoreVourcherSqlInstance.Inst.UpdateVourcher(request.VourcherId, request.VourcherName, request.VourcherType, request.CountUse, request.VourcherReward, request.VourcherDescription, request.thumbnail, request.Targets, request.StartTime, request.EndTime, request.Status, out responseStatus);

            if (responseStatus == EStatusCode.SUCCESS)
            {
                await RedisGatewayCacheManager.Inst.DeleteDataFromCacheAsync("AllVourcher").ConfigureAwait(false);
                await RedisGatewayCacheManager.Inst.DeleteDataFromCacheAsync("VourcherInfo:" + request.VourcherId).ConfigureAwait(false);
            }
            return Ok(new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = JsonConvert.SerializeObject(data) });
        }

        [HttpGet]
        [Route("GetAllVourcher")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> GetAllVourcher()
        {
            int responseStatus = 0;

            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });

            var data = StoreVourcherSqlInstance.Inst.GetAllVourcher();

            return Ok(new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = JsonConvert.SerializeObject( data) });
        }

        [HttpPost]
        [Route("AddVourcherToUser")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> AddVourcherToUser(RequestAddVourcherToUserCmsModel request)
        {
            int responseStatus = -99;

            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });

            responseStatus = StoreUsersSqlInstance.Inst.AddVourcherUser(request.AccountId, request.VourcherId, request.VourcherName, request.CountUse);
            if (responseStatus == EStatusCode.SUCCESS) {
                await RedisGatewayCacheManager.Inst.DeleteDataFromCacheAsync("AllVourcher").ConfigureAwait(false);
            }
            return Ok(new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus)});
        }

        [HttpPost]
        [Route("GetTopVourcher")]
        [ResponseCache(Duration = 10)]
        public async Task<IActionResult> GetTopVourcher(RequestGetMemberCmsModel request)
        {
            int responseStatus = 0;

            int checkRole = await _tokenManager.CheckRoleActionAsync(ERole.Administrator, Request);
            if (checkRole < 0)
                return Ok(new ResponseApiModel<string>() { Status = checkRole, Messenger = UltilsHelper.GetMessageByErrorCode(checkRole) });

            var data = StoreVourcherSqlInstance.Inst.GetTopVourcher(request.StartTime, request.EndTime);

            return Ok(new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = JsonConvert.SerializeObject(data) });
        }
    }
}
