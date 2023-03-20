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
            StoreMailConnectionString = "Data Source=" + ConfigDb.SQL_CONNECTION + ";Initial Catalog=Store.Email;Persist Security Info=True;" + ConfigDb.SQL_PASS;
            StoreBookConnectionString = "Data Source=" + ConfigDb.SQL_CONNECTION + ";Initial Catalog=Store.Book;Persist Security Info=True;" + ConfigDb.SQL_PASS;
            StoreMemberConnectionString = "Data Source=" + ConfigDb.SQL_CONNECTION + ";Initial Catalog=Store.Member;Persist Security Info=True;" + ConfigDb.SQL_PASS;
            StoreVourcherConnectionString = "Data Source=" + ConfigDb.SQL_CONNECTION + ";Initial Catalog=Store.Vourcher;Persist Security Info=True;" + ConfigDb.SQL_PASS;
            StoreOrderConnectionString = "Data Source=" + ConfigDb.SQL_CONNECTION + ";Initial Catalog=Store.Order;Persist Security Info=True;" + ConfigDb.SQL_PASS;
        }
        public static string StoreUsersConnectionString = "";
        public static string StoreMailConnectionString = "";
        public static string StoreBookConnectionString = "";
        public static string StoreMemberConnectionString = "";
        public static string StoreVourcherConnectionString = "";
        public static string StoreOrderConnectionString = "";

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
