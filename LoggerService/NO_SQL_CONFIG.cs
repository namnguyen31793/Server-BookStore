using ShareData.Game.Slot;
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
        public static readonly string API_LOG_NORMAL_BILLING_COLLECTION = "ApiNormalLog_Billing_Collection";
        public static readonly string API_LOG_NORMAL_CMS_COLLECTION = "ApiNormalLog_Cms_Collection";
        public static readonly string API_LOG_NORMAL_RECEIVE_LOBBY_COLLECTION = "ApiNormalLog_ReceiveLobby_Collection";
        public static readonly string API_LOG_NORMAL_RECEIVE_CONFIG_COLLECTION = "ApiNormalLog_ReceiveConfig_Collection";
        public static readonly string API_LOG_NORMAL_RECEIVE_API_COLLECTION = "ApiNormalLog_ReceiveApi_Collection";
        public static readonly string API_LOG_NORMAL_RECEIVE_LIVE_COLLECTION = "ApiNormalLog_ReceiveLive_Collection";

        public static readonly string API_LOG_ERROR_COLLECTION = "ApiErrorLog_Collection";
        public static readonly string API_LOG_DEBUG_COLLECTION = "ApiDebugLog_Collection";
        public static readonly string API_LOG_NORMAL_COLLECTION = "ApiNormalLog_Collection";
        #endregion

        #region T09
        public static readonly string T09_SYSTEM_DATABASE_NAME = "T09System";
        public static readonly string SLOTMACHINE_TUTIEN_JACKPOT_LOG_COLLECTION = "SlotMachine_TuTien_Jackpot_Log_Collection";
        public static readonly string SLOTMACHINE_TUTIEN_DETAIL_COLLECTION = "SlotMachine_TuTien_Detail_Collection";
        public static readonly string SLOTMACHINE_TUTIEN_SPECIAL_DETAIL_COLLECTION = "SlotMachine_TuTien_Special_Detail_Collection";
        public static readonly string SLOTMACHINE_TUTIEN_SHOP_EXCHANGE_HISTORY_COLLECTION = "SlotMachine_TuTien_Shop_Exchange_History_Collection";
        public static readonly string T09_EVENT_STORE_BUY_GIFTCODE_LOG_COLLECTION = "T09_Event_Tet_Buy_GiftCode_Log_Collection";


        public static readonly string SLOTMACHINE_PUNCH_CAT_DETAIL_COLLECTION = "SlotMachine_PunchCat_Detail_Collection";
        public static readonly string SLOTMACHINE_PUNCH_CAT_JACKPOT_FAIL_COLLECTION = "SlotMachine_PunchCat_Jackpot_Fail_Collection";
        public static readonly string SLOTMACHINE_PUNCH_CAT_JACKPOT_DETAIL_COLLECTION = "SlotMachine_PunchCat_Jackpot_Detail_Collection";
        public static readonly string SLOTMACHINE_PUNCH_CAT_SWAPCAT = "SlotMachine_PunchCat_SwapCat";

        #endregion

        public static string Get_Slot_DetailSpin_CollectionName(int GameType)
        {
            switch (GameType)
            {
                case (int)GAME_TYPE.TUTIEN:
                    return NO_SQL_CONFIG.SLOTMACHINE_TUTIEN_DETAIL_COLLECTION;
                case (int)GAME_TYPE.EVENT_TET_2023:
                    return NO_SQL_CONFIG.SLOTMACHINE_PUNCH_CAT_DETAIL_COLLECTION;
                default:
                    return "";
            }
        }
        public static string Get_Slot_Jackpot_Fail_CollectionName(int GameType)
        {
            switch (GameType)
            {
                case (int)GAME_TYPE.TUTIEN:
                    return NO_SQL_CONFIG.SLOTMACHINE_TUTIEN_JACKPOT_LOG_COLLECTION;
                case (int)GAME_TYPE.EVENT_TET_2023:
                    return NO_SQL_CONFIG.SLOTMACHINE_PUNCH_CAT_JACKPOT_FAIL_COLLECTION;
                default:
                    return "";
            }
        }
        public static string Get_Slot_Jackpot_Detail_CollectionName(int GameType)
        {
            switch (GameType)
            {
                case (int)GAME_TYPE.TUTIEN:
                    return NO_SQL_CONFIG.SLOTMACHINE_TUTIEN_JACKPOT_LOG_COLLECTION;
                case (int)GAME_TYPE.EVENT_TET_2023:
                    return NO_SQL_CONFIG.SLOTMACHINE_PUNCH_CAT_JACKPOT_DETAIL_COLLECTION;
                default:
                    return "";
            }
        }
    }
}
