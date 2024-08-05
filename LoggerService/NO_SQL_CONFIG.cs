using System;
using System.Collections.Generic;
using System.Text;

namespace LoggerService
{
    public static class NO_SQL_CONFIG
    {
        public static void Initialize(string IpAddress)
        {
            HOST_CONFIG = IpAddress;
        }
        public static void InitializeCMS(string IpAddress, string Ghn_Url, string Ghn_Token, string Ghn_Id, string from_name, string from_phone, string from_address, string from_ward_name, string from_district_name, string From_district_code, string from_province_name)
        {
            HOST_CONFIG = IpAddress;
            GHN_Url = Ghn_Url;
            GHN_Token = Ghn_Token;
            GHN_Id = Ghn_Id;
            From_name = from_name;
            From_phone = from_phone;
            From_address = from_address;
            From_ward_name = from_ward_name;
            From_district_name = from_district_name;
            from_district_code = int.Parse(From_district_code);
            From_province_name = from_province_name;
        }

        public static void InitializeNHANH( string url, string token, string appid, string BusinessId, string utmCampaign, string utmSource, string utmMedium)
        {
            NHANH_Url = url;
            NHANH_Token = token;
            NHANH_AppId = appid;
            NHANH_BusinessId = BusinessId;
            NHANH_utmCampaign = utmCampaign;
            NHANH_utmSource = utmSource;
            NHANH_utmMedium = utmMedium;
        }
        public static string HOST_CONFIG = "mongodb://localhost:27017";
        public static readonly string LOG_SYSTEM_DATABASE = "LogSystem";
        public static readonly string SLOT_MACHINE_SPIN_LOG_COLLECTION = "SlotMachine_SpinLog";

        #region GHN
        public static string GHN_Url = "";
        public static string GHN_Token = "";
        public static string GHN_Id = "";
        #endregion

        #region NHANH
        public static string NHANH_Url = "";
        public static string NHANH_Token = "";
        public static string NHANH_AppId = "";
        public static string NHANH_BusinessId = "";
        public static string NHANH_utmCampaign = "";
        public static string NHANH_utmSource = "";
        public static string NHANH_utmMedium = "";
        #endregion
        #region Adress
        public static string From_name = "";
        public static string From_phone = "";
        public static string From_address = "";
        public static string From_ward_name = "";
        public static string From_district_name = "";
        public static int from_district_code = 0;
        public static string From_province_name = "";
        #endregion

        #region Log
        public static readonly string API_LOG_SYSTEM_DATABASE_NAME = "ApiLogSystem";
        public static readonly string API_TRACKING_SYSTEM_DATABASE_NAME = "TrackingLogSystem";
        public static readonly string API_LOG_NORMAL_BILLING_COLLECTION = "ApiNormalLog_Billing_Collection";
        public static readonly string API_LOG_NORMAL_CMS_COLLECTION = "ApiNormalLog_Cms_Collection";

        public static readonly string API_LOG_ERROR_COLLECTION = "ApiErrorLog_Collection";
        public static readonly string API_LOG_DEBUG_COLLECTION = "ApiDebugLog_Collection";
        public static readonly string API_LOG_NORMAL_COLLECTION = "ApiNormalLog_Collection";
        public static readonly string API_LOG_BUY_BOOK_COLLECTION = "BuyBookLog_Collection";
        public static readonly string API_LOG_CCU_COLLECTION = "CcuLog_Collection";
        public static readonly string API_LOG_TRACKING_ACTION_HOME = "Tracking_Action_Home_Collection";
        public static readonly string API_LOG_TRACKING_ACTION_USER = "Tracking_Action_User_Collection";
        public static readonly string API_LOG_TRACKING_LISTEN_AUDIO = "Tracking_Action_Listen_Audio_Collection";
        public static readonly string API_LOG_TRACKING_FIND_BOOK = "Tracking_Action_Find_Book_Collection";
        public static readonly string API_LOG_TRACKING_ONLINE = "Tracking_Action_Online";
        public static readonly string API_LOG_TRACKING_ONLINE_BY_HOURS = "Tracking_Action_Online_By_Hours";
        #endregion

    }
}
