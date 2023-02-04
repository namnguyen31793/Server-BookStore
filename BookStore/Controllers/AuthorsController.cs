using DAO.DAOImp;
using LoggerService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RedisSystem;
using ShareData.API;
using ShareData.ErrorCode;
using System;
using System.Threading.Tasks;
using UtilsSystem.Utils;

namespace BookStore.Controllers
{
    [ApiVersion("1.0")]
    [Route("v1/Authors")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {

        private ILoggerManager _logger;
        public AuthorsController(ILoggerManager logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("{authorId}/GetAuthorInfo")]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetAuthorInfo(int authorId)
        {
            var response = new ResponseApiModel<string>() { Status = EStatusCode.SYSTEM_ERROR, Messenger = UltilsHelper.GetMessageByErrorCode(EStatusCode.SYSTEM_ERROR) };
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                string keyRedis = "CacheAuthorInfo:" + authorId;
                string jsonListSimpleBook = RedisGatewayManager<string>.Inst.GetDataFromCache(keyRedis);
                if (string.IsNullOrEmpty(jsonListSimpleBook))
                {
                    var listBook = StoreBookSqlInstance.Inst.GetAuthorById(authorId, out responseStatus);
                    if (responseStatus == EStatusCode.SUCCESS)
                    {
                        jsonListSimpleBook = JsonConvert.SerializeObject(listBook);
                        RedisGatewayManager<string>.Inst.SaveData(keyRedis, jsonListSimpleBook, 600);
                    }
                }
                response = new ResponseApiModel<string>() { Status = responseStatus, Messenger = UltilsHelper.GetMessageByErrorCode(responseStatus), DataResponse = jsonListSimpleBook };
            }
            catch (Exception ex)
            {
                await _logger.LogError("Books-GetListSimpleBookSameName{}", ex.ToString()).ConfigureAwait(false);
            }
            return Ok(response);
        }
    }
}
