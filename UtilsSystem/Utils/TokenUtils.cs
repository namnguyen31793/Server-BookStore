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

        public void AddTokenCache(string Token, string result, int timeExpires) {
            string keyRedis = "Token:" + Token;

            RedisGatewayCacheManager.Inst.SaveData(keyRedis, result, (int)(timeExpires / 60));
        }
    }
}
