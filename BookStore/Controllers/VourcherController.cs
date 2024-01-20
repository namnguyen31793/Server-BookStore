using BookStore.Instance;
using BookStore.Interfaces;
using BookStore.Utils;
using DAO.DAOImp;
using LoggerService;
using LoggerService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RedisSystem;
using ShareData.API;
using ShareData.ErrorCode;
using System.Threading.Tasks;
using UtilsSystem.Utils;

namespace BookStore.Controllers
{
    [ApiVersion("1.0")]
    [Route("v1/Vourchers")]
    [ApiController]
    public class VourcherController : ControllerBase
    {
        private IReportLog _report;
        private ILoggerManager _logger;
        private IEmailSender _emailSender;
        private ITokenManager _tokenManager;
        public VourcherController(ILoggerManager logger, IEmailSender emailSender, ITokenManager tokenManager, IReportLog reportManager)
        {
            _logger = logger;
            _emailSender = emailSender;
            _tokenManager = tokenManager;
            _report = reportManager;
        }
        private string token = string.Empty;

        [HttpGet]
        [Route("GetVourcherInfo")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> GetVourcherInfo(int vourcherId)
        {
            int responseStatus = -99;

            string keyRedis = "VourcherInfo:" + vourcherId;
            string jsonString = await RedisGatewayCacheManager.Inst.GetDataFromCacheAsync(keyRedis);
            if (string.IsNullOrEmpty(jsonString))
            {
                var listComment = StoreVourcherSqlInstance.Inst.GetVourcherById(vourcherId, ref responseStatus);
                if (responseStatus == EStatusCode.SUCCESS)
                {
                    jsonString = JsonConvert.SerializeObject(listComment);
                    await RedisGatewayCacheManager.Inst.SaveDataAsync(keyRedis, jsonString, 2);
                }
            }
            else
            {
                responseStatus = EStatusCode.SUCCESS;
            }
            return Ok(new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = jsonString });
        }

        [HttpGet]
        [Route("GetAllVourcher")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> GetAllVourcher()
        {
            int responseStatus = EStatusCode.SUCCESS;
            string keyRedis = "AllVourcher";
            string jsonString = await RedisGatewayCacheManager.Inst.GetDataFromCacheAsync(keyRedis);
            if (string.IsNullOrEmpty(jsonString))
            {
                var listComment = StoreVourcherSqlInstance.Inst.UserGetAllVourcher();
                jsonString = JsonConvert.SerializeObject(listComment);
                await RedisGatewayCacheManager.Inst.SaveDataAsync(keyRedis, jsonString, 2);
            }

            return Ok(new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = jsonString });
        }
    }
}
