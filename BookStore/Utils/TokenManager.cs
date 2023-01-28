using RedisSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UtilsSystem.Utils;

namespace BookStore.Utils
{
    public class TokenManager
    {
        public static string GenerateAccessToken(long accountId, ClientRequestInfo clientInfo)
        {
            var response = string.Format("{0}|{1}|{2}|{3}",
                                          DateTime.UtcNow.Ticks,
                                          accountId,
                                          clientInfo.MerchantId,
                                          clientInfo.DeviceId);
            response = Security.Encrypt(CONFIG.SecretTokenKey, response);
            string result = response.Replace("=", "_");

            string keyRedis = "Token:" + result;
            RedisGatewayCacheManager.Inst.SaveData(keyRedis, accountId.ToString(), 5);

            return result;
        }

        public static bool CheckAccessToken(string accessToken) {
            string keyRedis = "Token:" + accessToken;
            return RedisGatewayCacheManager.Inst.CheckExistKey(keyRedis);
        }

        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        public static string GetHash(string input)
        {
            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();
            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);
            return Convert.ToBase64String(byteHash);
        }
    }
}
