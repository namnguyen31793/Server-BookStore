using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.DB.Users
{
    public class AccountModelDb
    {
        public long AccountId { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public int UserRole { get; set; }
        public DateTime RegisterDate { get; set; }
        public string RegisterIp { get; set; }
        public string DeviceId { get; set; }
        public int BlockType { get; set; }
        public int IsValidate { get; set; }
        public string AvatarId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime BirthDay { get; set; }
        public string Adress { get; set; }
    }

    public class AccountModel
    {
        public long AccountId { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string AvatarId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime BirthDay { get; set; }
        public string Adress { get; set; }
        public AccountModel() {
        }
        public AccountModel(AccountModelDb model) {
            AccountId = model.AccountId;
            UserName = model.UserName;
            NickName = model.NickName;
            AvatarId = model.AvatarId;
            PhoneNumber = model.PhoneNumber;
            Email = model.Email;
            BirthDay = model.BirthDay;
            Adress = model.Adress;
        }
    }
}
