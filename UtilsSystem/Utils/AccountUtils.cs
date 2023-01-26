using ShareData.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UtilsSystem.Utils
{
    public class AccountUtils
    {
    /// <summary>
     /// 
     /// </summary>
     /// <param name="accountName"></param>
     /// <returns></returns>
        public static bool IsTrueAccountName(string accountName)
        {
            if (Regex.IsMatch(accountName, @"
            # Validate username with 5 constraints.
            ^                          # Anchor to start of string.
            # 1- only contains alphanumeric characters , underscore and dot.
            # 2- underscore and dot can't be at the end or start of username,
            # 3- underscore and dot can't come next to each other.
            # 4- each time just one occurrence of underscore or dot is valid.
            (?=[A-Za-z0-9]+(?:[_.][A-Za-z0-9]+)*$)
            # 5- number of characters must be between 8 to 20.
            [A-Za-z0-9_.]{6,16}        # Apply constraint 5.
            $                          # Anchor to end of string.
            ", RegexOptions.IgnorePatternWhitespace))
            {
                if (accountName.Contains("FB_")) return false;
                if (accountName.Contains("GG_")) return false;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// password can thoa man cac dieu kien :
        /// 1. co it nhat 1 so
        /// 2. co it nhat 1 ky tu thuong
        /// 3. co it nhat 1 ky tu hoa
        /// 4. co it nhat 8 ky tu va nhieu nhat 16 ky tu
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool IsTruePassword(string password)
        {
            var noSpace = new Regex("", RegexOptions.IgnorePatternWhitespace);
            var hasNumber = new Regex(@"[0-9]+");
            //var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMinimumMaximum = new Regex(@".{6,16}");
            return hasNumber.IsMatch(password) && hasMinimumMaximum.IsMatch(password) && noSpace.IsMatch(password);
        }

        public static bool IsTrueNickName(string nickName)
        {
            if (nickName.Contains(" ")) return false;
            if (nickName.Contains("|")) return false;
            if (nickName.Contains(",")) return false;
            if (nickName.Contains(";")) return false;
            if (nickName.ToLower().Contains("admin")) return false;
            return IsTrueAccountName(nickName);
        }

        public static string EncryptPasswordMd5(string password)
        {
            var dataEncrypt = StaticData.SecretPassKey + password;
            var response = Security.MD5Encrypt(dataEncrypt);
            return response;
        }
    }
}
