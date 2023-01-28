using RedisSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilsSystem.Utils
{
    public class TokenUtils
    {
        public bool CheckToken(string Token) {
            var isExpires = RedisGatewayCacheManager.Inst.CheckExistKey("Token:" + Token);
            return !isExpires;
        }

        public void AddTokenCache(string Token, byte[] result, int timeExpires)
        {
            string keyRedis = "Token:" + Token;
            RedisGatewayManager<GoProfile>.Inst.SaveByte(keyRedis, result, (int)(timeExpires / 60));
        }

        public void AddTokenCache(string Token, string result, int timeExpires) {
            string keyRedis = "Token:" + Token;

            RedisGatewayCacheManager.Inst.SaveData(keyRedis, result, (int)(timeExpires / 60));
        }

        public string GetInfobyToken(string Token) {
            string cache = RedisGatewayManager<GoProfile>.Inst.GetDataFromCache("Token:" + Token);
            return cache;
        }

        public string InitCookieByAccountId(int accountId, string MerchantId, string DeviceId)
        {
            var response = string.Format("{0}|{1}|{2}|{3}",
                                          DateTime.UtcNow.Ticks,
                                          accountId,
                                          MerchantId,
                                          DeviceId);
            response = Security.Encrypt(CONFIG.SecretTokenKey, response);
            string result = response.Replace("=", "_");
            return result;
        }
    }
}
