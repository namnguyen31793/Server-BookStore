using BookStore.Interfaces;
using BookStore.Utils;
using DAO.DAOImp;
using LoggerService;
using LoggerService.Interfaces;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RedisSystem;
using ShareData.API;
using ShareData.DB.Books;
using ShareData.ErrorCode;
using ShareData.Request;
using ShareData.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UtilsSystem.Utils;

namespace BookStore.Controllers
{
    [ApiVersion("1.0")]
    [Route("v1/Reports")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private ILoggerManager _logger;
        private IReportLog _report;
        public ReportsController(ILoggerManager logger, IReportLog reportLog)
        {
            _logger = logger;
            _report = reportLog;
        }

        [HttpPost]
        [Route("ActionUser")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> ActionUser(RequestTracking model)
        {
            try
            {
                await _report.TrackingActionUser(model.data);
                return Ok();
            }
            catch (Exception ex)
            {
                await _logger.LogError("Reports-ActionUser{}", ex.ToString()).ConfigureAwait(false);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("ActionHome")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> ActionHome(RequestTracking model)
        {
            try
            {
                await _report.TrackingActionHome(model.data);
                return Ok();
            }
            catch (Exception ex)
            {
                await _logger.LogError("Reports-ActionHome{}", ex.ToString()).ConfigureAwait(false);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("ActionFindBook")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> ActionFindBook(RequestTracking model)
        {
            try
            {
                await _report.TrackingFindBook(model.data);
                return Ok();
            }
            catch (Exception ex)
            {
                await _logger.LogError("Reports-ActionFindBook{}", ex.ToString()).ConfigureAwait(false);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("ListenAudio")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> ListenAudio(RequestTracking model)
        {
            try
            {
                await _report.TrackingListenAudio(model.data);
                return Ok();
            }
            catch (Exception ex)
            {
                await _logger.LogError("Reports-ListenAudio{}", ex.ToString()).ConfigureAwait(false);
                return BadRequest();
            }
        }
    }
}
