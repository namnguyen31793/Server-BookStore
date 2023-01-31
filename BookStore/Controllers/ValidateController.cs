using BookStore.Utils;
using DAO.DAOImp;
using LoggerService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShareData.API;
using ShareData.ErrorCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UtilsSystem.Utils;

namespace BookStore.Controllers
{
    [ApiVersion("1.0")]
    [Route("v1/Validate")]
    [ApiController]
    public class ValidateController : ControllerBase
    {
        private ILoggerManager _logger;
        public ValidateController(ILoggerManager logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("Email")]
        [ResponseCache(Duration = 5)]
        public async Task<IActionResult> ValidateEmail(string key)
        {
            string email = "";
            var accountid = TokenManager.ReadKeyTokenValidate(key, ref email);
            if (accountid < 0) {
                return Content(UltilsHelper.GetMessageByErrorCode((int)accountid) );
            }
            try
            {
                var responseStatus = StoreUsersDAO.Inst.ValidateUser(accountid);
            }
            catch (Exception ex)
            {
                await _logger.LogError("Account-Login{}", ex.ToString()).ConfigureAwait(false);
            }
            return Content(UltilsHelper.GetMessageByErrorCode(EStatusCode.SUCCESS);
        }
    }
}
