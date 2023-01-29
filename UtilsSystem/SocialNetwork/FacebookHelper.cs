using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using UtilsSystem.SocialNetwork;

namespace UtilsSystem.SocialNetwork
{
    public class FacebookHelper
    {

        public static async Task<string> GetFacebookUserIdAsync(string facebookToken)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(30000); //30s

            var request = new RestRequest(HttpUtility.UrlEncode("me?access_token=" +facebookToken), Method.Get);
            var client = new RestClient(HttpUtility.UrlEncode("https://graph.facebook.com/"));
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);
            var content = response.Content;
            if (content != null)
            {
                return content;
            }
            else
            {
                return string.Empty;
            }
        }

        public static async Task<string> GetFacebookUserNameAsync(string facebookToken)
        {
            //lay rieng
            var facebookAccountId = await GetFacebookUserIdAsync(facebookToken);
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
