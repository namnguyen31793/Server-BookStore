using Microsoft.AspNetCore.Http;
using RedisSystem;
using ShareData.ErrorCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UtilsSystem.Utils;

namespace BookStoreCMS.Utils
{
    public class TokenCMSManager
    {
        public static async Task<string> GenerateAccessTokenAsync(long accountId, int role, ClientRequestInfo clientInfo)
        {
            var response = string.Format("{0}|{1}|{2}|{3}|{4}",
                                          DateTime.UtcNow.Ticks,
                                          accountId,
                                          role,
                                          clientInfo.MerchantId,
                                          clientInfo.DeviceId);
            response = Security.Encrypt(CONFIG.SecretTokenKey, response);
            string result = response.Replace("=", "_");

            string keyRedis = "TokenCMS:" + result;
            await RedisGatewayCacheManager.Inst.SaveDataAsync(keyRedis, accountId.ToString() +"-"+role, 5);

            return result;
        }
        public static async Task<long> GetAccountIdByAccessTokenAsync(HttpRequest request)
        {
            int role = 0;
            string accessToken = "";
            if (request.Headers.TryGetValue("Authorization", out var values)) {
                accessToken = values.FirstOrDefault();
            }
            if (string.IsNullOrEmpty(accessToken))
                return EStatusCode.USER_NOT_LOGIN;

            string keyRedis = "TokenCMS:" + accessToken;
            long accountId = -1;
            var data = await RedisGatewayCacheManager.Inst.GetDataFromCacheAsync(keyRedis);
            if (string.IsNullOrEmpty(data))
            {
                var listdata = data.Split("-");
                long.TryParse(listdata[0], out accountId);
                if (accountId <= 0)
                    return EStatusCode.TOKEN_EXPIRES;
                int.TryParse(listdata[1], out role);
            }
            return accountId;
        }

        public static async Task<int> CheckRoleActionAsync(int rolePermission, HttpRequest request) {
            int response = EStatusCode.SYSTEM_ERROR;
            string accessToken = "";
            if (request.Headers.TryGetValue("Authorization", out var values))
            {
                accessToken = values.FirstOrDefault();
            }
            if (string.IsNullOrEmpty(accessToken))
                return EStatusCode.USER_NOT_LOGIN;

            string keyRedis = "TokenCMS:" + accessToken;
            long accountId = -1;
            int role = 0;
            var data = await RedisGatewayCacheManager.Inst.GetDataFromCacheAsync(keyRedis);
            if (!string.IsNullOrEmpty(data))
            {
                var listdata = data.Split("-");
                long.TryParse(listdata[0], out accountId);
                if (accountId <= 0)
                    return EStatusCode.TOKEN_EXPIRES;
                int.TryParse(listdata[1], out role);
                if (role > rolePermission)
                {
                    response = EStatusCode.ACCOUNT_NOT_ENOUGH_ROLE;
                }
                else {
                    response = EStatusCode.SUCCESS;
                }
            } else {
                response = EStatusCode.TOKEN_EXPIRES;
            }

            return response;
        }

        public static async Task<long> GetAccountIdByAccessTokenAsync(string accessToken) {
            string keyRedis = "Token:" + accessToken;
            long accountId = -1;
            var data = await RedisGatewayCacheManager.Inst.GetDataFromCacheAsync(keyRedis);
            long.TryParse(data, out accountId);
            return accountId;
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

        public static async Task<string> GenerateKeyTokenValidateAsync(long accountId, string email)
        {
            var response = string.Format("{0}|{1}|{2}",
                                          DateTime.UtcNow.Ticks,
                                          accountId,
                                          email);
            response = Security.Encrypt(CONFIG.SecretTokenKey, response);
            string result = response.Replace("=", "_");

            string keyRedis = "Token:" + result;
            await RedisGatewayCacheManager.Inst.SaveDataAsync(keyRedis, accountId.ToString(), 5);

            return result;
        }

        public static long ReadKeyTokenValidate(string accountInfoTxtRaw, ref string email)
        {
            try
            {
                accountInfoTxtRaw = accountInfoTxtRaw.Replace("_", "=");
                var accountInfoTxt = Security.Decrypt(CONFIG.SecretTokenKey, accountInfoTxtRaw);
                if (!IsCookieExpired(accountInfoTxt)) 
                    return EStatusCode.TOKEN_EXPIRES;
                var data = accountInfoTxt.Split('|');
                long accountId = Int64.Parse(data[1]);
                email = data[2];
                return accountId;
            }
            catch (Exception ex)
            {
                return EStatusCode.DATA_INVAILD;
            }
        }
        private static bool IsCookieExpired(string accountInfoTxt)
        {
            var accounts = accountInfoTxt.Split('|');
            var timeCreateTicks = Int64.Parse(accounts[0]);
            var timeCreate = new DateTime(timeCreateTicks);
            var dateTimeNow = DateTime.UtcNow;
            var totalSecond = (dateTimeNow - timeCreate).TotalSeconds;
            if (totalSecond > 600) return false; //cookie het han 10p
            return true;
        }
    }
}
