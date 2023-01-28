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

            StoreUsersConnectionString = "Data Source=" + ConfigDb.SQL_CONNECTION + ";Initial Catalog=Store.Users;Persist Security Info=True;" + ConfigDb.SQL_PASS;
        }
        public static string StoreUsersConnectionString = "";

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
