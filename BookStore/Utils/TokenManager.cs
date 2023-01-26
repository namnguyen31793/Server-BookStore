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
        public static string InitCookieByAccountId(int accountId, ClientRequestInfo clientInfo)
        {
            var response = string.Format("{0}|{1}|{2}|{3}",
                                          DateTime.UtcNow.Ticks,
                                          accountId,
                                          clientInfo.MerchantId,
                                          clientInfo.DeviceId);
            response = Security.Encrypt(CONFIG.SecretTokenKey, response);
            string result = response.Replace("=", "_");
            return result;
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
