using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace UtilsSystem.SocialNetwork
{
    public class GoogleHelper
    {
        public static async Task<string> GetGoogleUserIdAsync(string access_token)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(30000); //30s

            var request = new RestRequest("", Method.Get);
            request.AddHeader("Content-Type", "application/json");
            var client = new RestClient("https://www.googleapis.com/plus/v1/people/me?access_token=" + access_token);
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
        public static async Task<string> GetGoogleUserNameAsync(string access_token)
        {
            //lay rieng
            var facebookAccountId = await GetGoogleUserIdAsync(access_token);
            var modelToken = JsonConvert.DeserializeObject<TokenGoogleModel>(facebookAccountId);
            if (modelToken != null)
                return "GG_" + modelToken.sub;
            return string.Empty;
        }
    }
    public class TokenGoogleModel
    {
        public string sub { get; set; }
        public string name { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string email { get; set; }
        public string locale { get; set; }
    }
}
