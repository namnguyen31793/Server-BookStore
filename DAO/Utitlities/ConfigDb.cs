using System;
using System.Collections.Generic;
using System.Text;

namespace DAO.Utitlities
{
    public static class ConfigDb
    {
        public static void Initialize(string DbConfig, string SqlPass)
        {
            ConfigDb.SQL_CONNECTION = $@"{ DbConfig }";
            ConfigDb.SQL_PASS = SqlPass;

            LauncherLobbyConnectionString = "Data Source=" + ConfigDb.SQL_CONNECTION + ";Initial Catalog=Launcher.LobbyGame;Persist Security Info=True;" + ConfigDb.SQL_PASS;
            LauncherConfigConnectionString = "Data Source=" + ConfigDb.SQL_CONNECTION + ";Initial Catalog=Launcher.Config;Persist Security Info=True;" + ConfigDb.SQL_PASS;
            SystemFriendConnectionString = "Data Source=" + ConfigDb.SQL_CONNECTION + ";Initial Catalog=System.Friends;Persist Security Info=True;" + ConfigDb.SQL_PASS;
        
            CmsLogConnectionString = "Data Source=" + ConfigDb.SQL_CONNECTION + ";Initial Catalog=CmsLog;Persist Security Info=True;" + ConfigDb.SQL_PASS;
            LauncherBillingConnectionString = "Data Source=" + ConfigDb.SQL_CONNECTION + ";Initial Catalog=Launcher.Billing;Persist Security Info=True;" + ConfigDb.SQL_PASS;
            LauncherSubcribeConnectionString = "Data Source=" + ConfigDb.SQL_CONNECTION + ";Initial Catalog=Launcher.Subscribe;Persist Security Info=True;" + ConfigDb.SQL_PASS;
            LauncherLiveConnectionString = "Data Source=" + ConfigDb.SQL_CONNECTION + ";Initial Catalog=Launcher.Live;Persist Security Info=True;" + ConfigDb.SQL_PASS;
            LauncherAccountStoreConnectionString = "Data Source=" + ConfigDb.SQL_CONNECTION + ";Initial Catalog=Launcher.AccountStore;Persist Security Info=True;" + ConfigDb.SQL_PASS;
            //slot
            SlotMachineTuTien = "Data Source=" + ConfigDb.SQL_CONNECTION + ";Initial Catalog=NewSlotMachine.TuTien;Persist Security Info=True;" + ConfigDb.SQL_PASS;
            SlotMachineTuTienSpecial = "Data Source=" + ConfigDb.SQL_CONNECTION + ";Initial Catalog=NewSlotMachine.TuTienSpecial;Persist Security Info=True;" + ConfigDb.SQL_PASS;
            SlotMachineEvent = "Data Source=" + ConfigDb.SQL_CONNECTION + ";Initial Catalog=NewSlotMachine.Event;Persist Security Info=True;" + ConfigDb.SQL_PASS;
            LauncherNotificationConnectionString = "Data Source=" + ConfigDb.SQL_CONNECTION + ";Initial Catalog=Launcher.Notification;Persist Security Info=True;" + ConfigDb.SQL_PASS;
            SlotMachinePunchCat = "Data Source=" + ConfigDb.SQL_CONNECTION + ";Initial Catalog=NewSlotMachine.PunchCat;Persist Security Info=True;" + ConfigDb.SQL_PASS;
        }

        public static string DbLogConnectionString = "";
        public static string ApiLogicConnectionString = "";
        public static string CmsConnectionString = "";
        public static string TrackingConnectionString = "";
        public static string BillingConnectionString = "";
        public static string EmailConnectionString = "";
        public static string EventConfigConnectionString = "";
        public static string IapConnectionString = "";
        public static string CMSConfigConnectionString = "";

        public static string LauncherLobbyConnectionString = "";
        public static string LauncherConfigConnectionString = "";
        public static string SystemFriendConnectionString = "";
        //new
        public static string CmsLogConnectionString = "";
        public static string LauncherBillingConnectionString = "";
        public static string LauncherSubcribeConnectionString = "";
        public static string LauncherLiveConnectionString = "";
        public static string LauncherAccountStoreConnectionString = "";
        //slot
        public static string SlotMachineTuTien = "";
        public static string SlotMachineTuTienSpecial = "";
        public static string SlotMachineEvent = "";
        public static string LauncherNotificationConnectionString = "";
        public static string SlotMachinePunchCat = "";

        public static string SQL_CONNECTION;

        public static string SQL_PASS;
        //public static string SQL_CONNECTION
        //{
        //    get
        //    {
        //        return @"VM-36";
        //        //return @"127.0.0.1\NEWSQLINSTANCE";
        //    }
        //}

        //public static string SQL_PASS
        //{
        //    get
        //    {
        //        return "User ID=allvalk;Password=Vinh312@";
        //    }
        //}
    }
}
