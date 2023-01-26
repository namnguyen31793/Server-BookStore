using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace RedisSystem.Config
{
    public static class RedisConfig
    {
        public static void Initialize(string IpAddress, string Port, string Pass)
        {
            RedisServerIpAddress = IpAddress;
            RedisServerPort = Port;
            Password = Pass;
        }

        public static string RedisServerIpAddress ;
        public static string RedisServerPort;
        public static string Password;

        #region KEY
        public static string CcuKey = "SystemInfo:CCU";
        #endregion


    }
}
