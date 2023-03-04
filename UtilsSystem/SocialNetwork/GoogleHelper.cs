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
            var client = new RestClient("https://www.googleapis.com/oauth2/v3/userinfo?access_token=" + access_token);
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
            var ggAccountId = await GetGoogleUserIdAsync(access_token);
            var modelToken = JsonConvert.DeserializeObject<TokenGoogleModel>(ggAccountId);
            if (modelToken != null && !string.IsNullOrEmpty(modelToken.sub))
                return "GG_" + modelToken.sub;
            return string.Empty;
        }
        public static async Task<string> GetGoogleUserIdByJwtAsync(string Jwt)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(30000); //30s

            var request = new RestRequest("", Method.Get);
            request.AddHeader("Content-Type", "application/json");
            var client = new RestClient("https://oauth2.googleapis.com/tokeninfo?id_token=" + Jwt);
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
        public static async Task<string> GetGoogleUserNameByJwtAsync(string jwt)
        {
            //lay rieng
            var ggAccountId = await GetGoogleUserIdByJwtAsync(jwt);
            var modelToken = JsonConvert.DeserializeObject<TokenGoogleModel2>(ggAccountId);
            if (modelToken != null && !string.IsNullOrEmpty(modelToken.sub))
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
    public class TokenGoogleModel2
    {
        public string iss { get; set; }
        public string azp { get; set; }
        public string aud { get; set; }
        public string sub { get; set; }
        public string email { get; set; }
        public string email_verified { get; set; }
        public string name { get; set; }
        public string picture { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string locale { get; set; }
    }
}
