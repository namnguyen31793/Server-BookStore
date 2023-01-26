using ShareData.DAO;
using System;

namespace ShareData.API
{
    public class AccountModel
    {
        public int AccountId { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public int AvatarId { get; set; }
        public long IngameBalance { get; set; }
        public DateTime BirthDay { get; set; }
        public string AccountLinker { get; set; }
        public int MerchantId { get; set; }
        public string DeviceId { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime LastLoginTime { get;  set; }
        public long DifferenceCashOutCashinIn { get; set; }
        public int IsValidate { get; set; }
        public int VipLevel { get; set; }
        public long VipPoint { get; set; }
        public string Cookie { get; set; }

        public static AccountModel GetClientAccountModel(RawAccountModel rawAccountModel, long accountBalance, string cookie)
        {
            if (rawAccountModel == null || rawAccountModel.AccountId == 0) return new AccountModel();
            return new AccountModel()
            {
                AccountId = rawAccountModel.AccountId,
                UserName = rawAccountModel.UserName,
                NickName = rawAccountModel.NickName,
                IngameBalance = accountBalance,
                AvatarId = rawAccountModel.AvatarId,
                BirthDay = rawAccountModel.BirthDay,
                AccountLinker = rawAccountModel.AccountLinker,
                PhoneNumber = rawAccountModel.PhoneNumber,
                DeviceId = string.Empty,
                MerchantId = rawAccountModel.MerchantId,
                RegisterDate = rawAccountModel.RegisterDate,
                LastLoginTime = rawAccountModel.LastLoginTime,
                DifferenceCashOutCashinIn = rawAccountModel.DifferenceCashOutCashinIn,
                IsValidate = rawAccountModel.IsValidate,
                Cookie = cookie
            };
        }
        public static AccountModel GetClientAccountModel(RawAccountModel rawAccountModel)
        {
            if (rawAccountModel == null || rawAccountModel.AccountId == 0) return new AccountModel();
            return new AccountModel()
            {
                AccountId = rawAccountModel.AccountId,
                UserName = rawAccountModel.UserName,
                NickName = rawAccountModel.NickName,
                AvatarId = rawAccountModel.AvatarId,
                BirthDay = rawAccountModel.BirthDay,
                AccountLinker = rawAccountModel.AccountLinker,
                PhoneNumber = rawAccountModel.PhoneNumber,
                DeviceId = string.Empty,
                MerchantId = rawAccountModel.MerchantId,
                RegisterDate = rawAccountModel.RegisterDate,
                LastLoginTime = rawAccountModel.LastLoginTime,
                DifferenceCashOutCashinIn = rawAccountModel.DifferenceCashOutCashinIn,
                IsValidate = rawAccountModel.IsValidate,
            };
        }
    }
}
