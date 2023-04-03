using LoggerService;
using RestSharp;
using ShareData.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace BookStoreCMS.Instance
{
    public class GhnInstance
    {
        private static GhnInstance _inst;
        private static object _syncObject = new object();

        public static GhnInstance Inst
        {
            get
            {
                if (_inst == null)
                    lock (_syncObject)
                    {
                        if (_inst == null) _inst = new GhnInstance();
                    }
                return _inst;
            }
        }

        public async Task<string> SendGetserviceIdGhn(int fromdistrict, int todistrict) {

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(30000); //30s

            var request = new RestRequest(HttpUtility.UrlEncode( "available-services?token=" + NO_SQL_CONFIG.GHN_Token+ "&from_district="+ fromdistrict + "&to_district=" + todistrict + "&shop_id=" + NO_SQL_CONFIG.GHN_Id), Method.Get);
            
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("token", NO_SQL_CONFIG.GHN_Token);
            request.AddHeader("shop_id", NO_SQL_CONFIG.GHN_Id);
            var client = new RestClient(NO_SQL_CONFIG.GHN_Url + "/shiip/public-api/v2/shipping-order/");
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

        public async Task<string> SendCreateOrderGhn(string note, string to_name, string to_phone, string to_address, string to_ward_name, string to_district_name, string to_province_name, long cod_amount, string content_note, int weight, int length, int width, int height
            , ItemGhnModel[] item, long service_id, long service_type_id)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(30000); //30s

            var request = new RestRequest("create", Method.Post); 
            request.AddJsonBody(new
            {
                payment_type_id = 2,
                note = note,
                from_name = NO_SQL_CONFIG.From_name,
                from_phone = NO_SQL_CONFIG.From_phone,
                from_address = NO_SQL_CONFIG.From_address,
                from_ward_name = NO_SQL_CONFIG.From_ward_name,
                from_district_name = NO_SQL_CONFIG.From_district_name,
                from_province_name = NO_SQL_CONFIG.From_province_name,
                required_note = "KHONGCHOXEMHANG",
                return_name = NO_SQL_CONFIG.From_name,
                return_phone = NO_SQL_CONFIG.From_phone,
                return_address = NO_SQL_CONFIG.From_address,
                return_ward_name = NO_SQL_CONFIG.From_ward_name,
                return_district_name = NO_SQL_CONFIG.From_district_name,
                return_province_name = NO_SQL_CONFIG.From_province_name,
                client_order_code = "",
                to_name = to_name,
                to_phone = to_phone,
                to_address = to_address,
                to_ward_name = to_ward_name,
                to_district_name = to_district_name,
                to_province_name = to_province_name,
                cod_amount = cod_amount,
                content = content_note,
                weight = weight,
                length = length,
                width = width,
                height = height,
                cod_failed_amount = 2000,
                pick_station_id = 1444,
                insurance_value = cod_amount,
                service_id = service_id,
                service_type_id = service_type_id,
                pickup_time = 1665272576,
                items = item,
            });
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("token", NO_SQL_CONFIG.GHN_Token);
            request.AddHeader("shop_id", NO_SQL_CONFIG.GHN_Id);
            var client = new RestClient(NO_SQL_CONFIG.GHN_Url+ "shiip/public-api/v2/shipping-order/");
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
    }
}
