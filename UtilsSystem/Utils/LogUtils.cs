using RedisSystem;
using RedisSystem.Config;
using ShareData.LogSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilsSystem.Utils
{
    public class LogUtils
    {
        private static readonly object SyncObject = new object();

        private static LogUtils _inst;

        public static LogUtils Inst
        {
            get
            {
                if (_inst == null)
                {
                    lock (SyncObject)
                    {
                        if (_inst == null)
                            _inst = new LogUtils();
                    }
                }
                return _inst;
            }
        }

        public void SaveListLog(string log, API_LOG_TYPE logServies)
        {
            //odd and even
            var isOdd = (int)(DateTime.UtcNow.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMinutes) % 2;
            if (isOdd == 1)
            {
                //add odd
                string keyRedis = "Mongo:LogOdd-"+ logServies+ ":Id-" + DateTime.Now.Ticks;
                RedisGatewayCacheManager.Inst.SaveData(keyRedis, log, 10);
            }
            else
            {
                //add event
                string keyRedis = "Mongo:LogEven-" + logServies + ":Id-" + DateTime.Now.Ticks;
                RedisGatewayCacheManager.Inst.SaveData(keyRedis, log, 10);
            }
        }
    }
}
