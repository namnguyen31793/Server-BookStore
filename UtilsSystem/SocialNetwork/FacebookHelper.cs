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

            var request = new RestRequest("", Method.Get); 
            request.AddHeader("Content-Type", "application/json");
            var client = new RestClient("https://graph.facebook.com/me?access_token=" + facebookToken);
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
            var modelToken = JsonConvert.DeserializeObject<TokenFbModel>(facebookAccountId);
            if(modelToken != null)
             return "FB_" + modelToken.id;
            return string.Empty;
        }

        public static async Task<TokenFbModel> GetFacebookModelAsync(string facebookToken)
        {
            //lay rieng
            var facebookAccountId = await GetFacebookUserIdAsync(facebookToken);
            var modelToken = JsonConvert.DeserializeObject<TokenFbModel>(facebookAccountId);
            if (modelToken != null)
                return modelToken;
            return null;
        }

        //public static string GetFacebookUserNam

        public static string GetMd5Password(string userName)
        {
            var password = userName + "Facebook@";
            return password;
        }

        public class TokenFbModel {
            public string name { get; set; }
            public string id { get; set; }
        }
    }
}
