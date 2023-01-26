using ShareData.Language;
using ShareData.LogSystem;

namespace UtilsSystem.Utils
{
    public class LanguageHelper
    {
        public static string GetCustomErrorMessage(long errorCode, ElanguageType language)
        {
            string response = string.Empty;
            return response;
        }
        //None = 0, Vni = 1, Eng = 2, Cam = 3
        public static string GetLoginMultiLanguage(string nickName, string language)
        {
            if (string.IsNullOrEmpty(nickName)) nickName = "PLAYER";
            var response = string.Empty;
            if (language.ToLower().Equals("vni"))
                return string.Format("Chào <color=#ffff00ff>{0}</color>, Chúc bạn chơi game vui vẻ !", nickName);
            else if (language.ToLower().Equals("eng"))
                return string.Format("Welcome <color=#ffff00ff>{0}</color>, Happy gaming !", nickName);
            else if (language.ToLower().Equals("cam"))
                return string.Format("សួស្តី <color=#ffff00ff>{0}</color>, លេងហ្គេមសប្បាយ !", nickName);
            else
            {
                return string.Format("Welcome <color=#ffff00ff>{0}</color>, Happy gaming !", nickName);
            }
        }

        public static string GetCreateIapOrderFail(string language)
        {
            var response = string.Empty;
            if (language.ToLower().Equals("vni"))
                return string.Format("Không thể mua vật phẩm");
            else if (language.ToLower().Equals("eng"))
                return string.Format("Can not buy items");
            else if (language.ToLower().Equals("cam"))
                return string.Format("មិនអាចទិញបាន");
            else
            {
                return string.Format("Can not buy items !");
            }
        }
    }
}
