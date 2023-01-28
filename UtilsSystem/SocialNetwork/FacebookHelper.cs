using Facebook;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UtilsSystem.SocialNetwork;

namespace UtilsSystem.SocialNetwork
{
    public class FacebookHelper
    {

        public static string GetFacebookUserId(string facebookToken)
        {
            var facebookClient = new FacebookClient(facebookToken);
            var me = facebookClient.Get("me") as JsonObject;
            if (me != null)
            {
                var uid = me["id"];
                return uid.ToString();
            }
            else
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// Lấy Fb Id -- Hot fix trường hợp business : nhiều appfacebook dùng chung 1 db (27-12-2017)
        /// TODO Cách tốt nhất là nên dùng 'token_for_business' để phân biệt
        /// </summary>
        /// <param name="facebookToken"></param>
        /// <returns></returns>
        public static string GetFacebookBusinessToken(string facebookToken)
        {
            var response = "";
            try
            {
                var facebookClient = new FacebookClient(facebookToken);
                var mes = facebookClient.Get("me?fields=token_for_business").ToString();
                var mesInfo = JsonConvert.DeserializeObject<FacebookBusinessResponse>(mes);
                //lấy UID app 1
                if (mesInfo != null)
                {
                    response = mesInfo.token_for_business;
                }
                else
                {
                    //Thường là Token hết hạn
                }
            }
            catch (Exception exception)
            {
                //Thường là mất kết nối tới Fb
                //NLogManager.PublishException(exception);
            }
            return response;
        }

        public static string GetFacebookUserName(string facebookToken)
        {
            //lay rieng
            var facebookAccountId = GetFacebookUserId(facebookToken);
            if (!string.IsNullOrEmpty(facebookAccountId)) return "FB_" + facebookAccountId;
            return string.Empty;
        }

        //public static string GetFacebookUserNam

        public static string GetFacebookPassword(string userName)
        {
            var password = userName + "Facebook@";
            return password;
        }
    }
}
