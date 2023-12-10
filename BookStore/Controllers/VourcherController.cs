using BookStore.Instance;
using BookStore.Interfaces;
using BookStore.Utils;
using DAO.DAOImp;
using LoggerService;
using LoggerService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShareData.API;
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

            var data = StoreVourcherSqlInstance.Inst.GetVourcherById(vourcherId, ref responseStatus);

            return Ok(new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = JsonConvert.SerializeObject(data) });
        }
    }
}
