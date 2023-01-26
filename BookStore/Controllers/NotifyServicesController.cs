using DAO.DAOImp;
using LoggerService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RedisSystem;
using ShareData.API;
using ShareData.DAO;
using ShareData.ErrorCode;
using ShareData.RequestCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServerEventTet2023.Utils;

namespace ServerEventTet2023.Controllers
{
    [ApiVersion("1.0")]
    [Route("v1/NotifyServices")]
    [ApiController]
    public class NotifyServicesController : ControllerBase
    {
        private ILoggerManager _logger;
        public NotifyServicesController(ILoggerManager logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("GetNotifyAdmin")]
        [ResponseCache(Duration = 5)]
        public async Task<IActionResult> GetNotifyAdmin()
        {
            var data = "";
            try
            {
                data = await Task.Run(() =>
                {
                    return new NotifyUtils().GetNotifyAdmin("Tet2023");
                });
            }
            catch (Exception ex)
            {
                await _logger.LogError("NotifyServices-GetNotifyAdmin{}", ex.ToString()).ConfigureAwait(false);
            }

            return Ok(new ResponseApiLauncher<string>() { Status = EStatusCode.SUCCESS, Messenger = UtilsSystem.Utils.UltilsHelper.GetMessageByErrorCode(EStatusCode.SUCCESS), DataResponse = data });
        }

    }
}
