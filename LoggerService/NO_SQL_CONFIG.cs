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
        public static string HOST_CONFIG = "mongodb://localhost:27017";
        public static readonly string LOG_SYSTEM_DATABASE = "LogSystem";
        public static readonly string SLOT_MACHINE_SPIN_LOG_COLLECTION = "SlotMachine_SpinLog";

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
        #endregion

    }
}
