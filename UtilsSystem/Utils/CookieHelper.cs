using ShareData.API;
using ShareData.DAO;
using ShareData.Helper;
using ShareData.LogSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilsSystem.Utils
{
    public class CookieHelper
    {
        public static string InitCookie(RawAccountModel accountModel, ClientInfo clientInfo)
        {
            var accountInfo = AccountModel.GetClientAccountModel(accountModel);
            var response = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}|{16}",
                                          DateTime.UtcNow.Ticks,
                                          accountInfo.AccountId,
                                          accountInfo.UserName,
                                          accountInfo.NickName,
                                          accountInfo.AvatarId,
                                          accountInfo.IngameBalance,
                                          accountInfo.BirthDay,
                                          clientInfo.MerchantId,
                                          accountInfo.AccountLinker,
                                          clientInfo.DeviceId,
                                          accountInfo.PhoneNumber,
                                          accountInfo.RegisterDate,
                                          accountInfo.LastLoginTime,
                                          accountInfo.DifferenceCashOutCashinIn,
                                          accountInfo.VipLevel,
                                          accountInfo.VipPoint);
            response = Security.Encrypt(StaticData.SecretCookieKey, response);
            string result = response.Replace("=", "_");
            return result;
        }

        public static AccountSystemModel GetAccountInfo(string accountInfoTxtRaw)
        {
            try
            {
                accountInfoTxtRaw = accountInfoTxtRaw.Split(',')[0];

                accountInfoTxtRaw = accountInfoTxtRaw.Replace("_", "=");
                var accountInfoTxt = Security.Decrypt(StaticData.SecretCookieKey, accountInfoTxtRaw);
                if (!IsCookieExpired(accountInfoTxt)) return new AccountSystemModel();
                var accounts = accountInfoTxt.Split('|');
                var accountInfo = new AccountSystemModel()
                {
                    AccountID = accounts[1],
                    NickName = accounts[2],
                    UserName = accounts[3],
                };
                return accountInfo;
            }
            catch (Exception)
            {
                return new AccountSystemModel();
            }
        }

        public static string InitCookie(string accountId, string nickName, string userName, ClientInfo clientInfo)
        {
            var response = string.Format("{0}|{1}|{2}|{3}",
                                          DateTime.UtcNow.Ticks,
                                          accountId,
                                          nickName,
                                          userName);
            response = Security.Encrypt(StaticData.SecretCookieKey, response);
            string result = response.Replace("=", "_");
            return result;
        }

        public static int GetAccountIdByCookie(string accountInfoTxtRaw)
        {
            try
            {
                accountInfoTxtRaw = accountInfoTxtRaw.Split(',')[0];

                accountInfoTxtRaw = accountInfoTxtRaw.Replace("_", "=");
                var accountInfoTxt = Security.Decrypt(StaticData.SecretCookieKey, accountInfoTxtRaw);
                if (!IsCookieExpired(accountInfoTxt)) return -1;
                var accounts = accountInfoTxt.Split('|');
                int accountId = Int32.Parse(accounts[1]);


                return accountId;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        private static bool IsCookieExpired(string accountInfoTxt)
        {
            var accounts = accountInfoTxt.Split('|');
            var timeCreateTicks = Int64.Parse(accounts[0]);
            var timeCreate = new DateTime(timeCreateTicks);
            var dateTimeNow = DateTime.UtcNow;
            var totalSecond = (dateTimeNow - timeCreate).TotalSeconds;
            if (totalSecond > StaticData.TimeCookieExpire) return false;//cookie het han
            return true;
        }
    }
}
