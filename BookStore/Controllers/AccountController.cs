using LoggerService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShareData.API;
using ShareData.ErrorCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Controllers
{
    [ApiVersion("1.0")]
    [Route("v1/Accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        
        private ILoggerManager _logger;
        public AccountController(ILoggerManager logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("Login")]
        [ResponseCache(Duration = 5)]
        public async Task<IActionResult> Login()
        {
            var data = "";
            try
            {
            }
            catch (Exception ex)
            {
                await _logger.LogError("NotifyServices-GetNotifyAdmin{}", ex.ToString()).ConfigureAwait(false);
            }

            return Ok(new ResponseApiLauncher<string>() { Status = EStatusCode.SUCCESS, Messenger = UtilsSystem.Utils.UltilsHelper.GetMessageByErrorCode(EStatusCode.SUCCESS), DataResponse = data });
        }

        [HttpPost]
        [Route("Regis")]
        [ResponseCache(Duration = 5)]
        public async Task<IActionResult> Regis()
        {
            var data = "";
            try
            {
            }
            catch (Exception ex)
            {
                await _logger.LogError("NotifyServices-GetNotifyAdmin{}", ex.ToString()).ConfigureAwait(false);
            }

            return Ok(new ResponseApiLauncher<string>() { Status = EStatusCode.SUCCESS, Messenger = UtilsSystem.Utils.UltilsHelper.GetMessageByErrorCode(EStatusCode.SUCCESS), DataResponse = data });
        }

        [HttpGet]
        [Route("RefreshToken")]
        [ResponseCache(Duration = 5)]
        public async Task<IActionResult> RefreshToken()
        {
            var data = "";
            try
            {
            }
            catch (Exception ex)
            {
                await _logger.LogError("NotifyServices-GetNotifyAdmin{}", ex.ToString()).ConfigureAwait(false);
            }

            return Ok(new ResponseApiLauncher<string>() { Status = EStatusCode.SUCCESS, Messenger = UtilsSystem.Utils.UltilsHelper.GetMessageByErrorCode(EStatusCode.SUCCESS), DataResponse = data });
        }

        [HttpGet]
        [Route("GetAccountInfo")]
        [ResponseCache(Duration = 5)]
        public async Task<IActionResult> GetAccountInfo()
        {
            var data = "";
            try
            {
            }
            catch (Exception ex)
            {
                await _logger.LogError("NotifyServices-GetNotifyAdmin{}", ex.ToString()).ConfigureAwait(false);
            }

            return Ok(new ResponseApiLauncher<string>() { Status = EStatusCode.SUCCESS, Messenger = UtilsSystem.Utils.UltilsHelper.GetMessageByErrorCode(EStatusCode.SUCCESS), DataResponse = data });
        }

        [HttpGet]
        [Route("GetListBook")]
        [ResponseCache(Duration = 5)]
        public async Task<IActionResult> GetListBook()
        {
            var data = "";
            try
            {
            }
            catch (Exception ex)
            {
                await _logger.LogError("NotifyServices-GetNotifyAdmin{}", ex.ToString()).ConfigureAwait(false);
            }

            return Ok(new ResponseApiLauncher<string>() { Status = EStatusCode.SUCCESS, Messenger = UtilsSystem.Utils.UltilsHelper.GetMessageByErrorCode(EStatusCode.SUCCESS), DataResponse = data });
        }
    }
}
