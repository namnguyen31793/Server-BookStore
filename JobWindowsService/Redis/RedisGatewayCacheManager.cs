using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobWindowsService.Redis
{
    public class RedisGatewayCacheManager
    {
        private static readonly object SyncObject = new object();

        private static RedisGatewayCacheManager _inst;

        public static RedisGatewayCacheManager Inst
        {
            get
            {
                if (_inst == null)
                {
                    lock (SyncObject)
                    {
                        if (_inst == null)
                            _inst = new RedisGatewayCacheManager();
                    }
                }
                return _inst;
            }
        }

        private Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            string cacheConnection = "127.0.0.1" + ":" + "6379" + ", password=" + "a123456" + ",abortConnect=false";
            return ConnectionMultiplexer.Connect(cacheConnection);
        });

        public ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }


        public bool StoreData(string key, string value, long minuteExpires = -1)
        {
            try
            {
                var _database = Connection.GetDatabase();
                if (minuteExpires < 0)
                    return _database.StringSet(key, value);
                var timeExpires = TimeSpan.FromMinutes(minuteExpires);
                return _database.StringSet(key, value, timeExpires);
            }
            catch (Exception exception)
            {
                return false;
            }
        }

        public string GetDataFromCache(string key)
        {
            try
            {
                var _database = Connection.GetDatabase();
                return _database.StringGet(key);
            }
            catch (Exception exception)
            {
                return "";
            }
        }

        public void DeleteDataFromCache(string key)
        {
            try
            {
                var _database = Connection.GetDatabase();
                _database.KeyDelete(key);
            }
            catch (Exception exception)
            {
            }
        }

        public bool CheckExistKey(string key)
        {
            try
            {
                var _database = Connection.GetDatabase();
                return _database.KeyExists(key);
            }
            catch (Exception exception)
            {
                return false;
            }
        }

        public void DeleteArrayKey(string forderName)
        {
            try
            {
                var keys = Connection.GetServer("127.0.0.1" + ":" + "6379").Keys();
                RedisKey[] keysArr = keys.Where(x => x.ToString().Contains(forderName)).Select(key => key).ToArray();
                Connection.GetDatabase().KeyDelete(keysArr);
            }
            catch (Exception exception)
            {
            }
        }

        public long GetCountByKey(string forderName)
        {
            long count = 0;
            try
            {
                var keys = Connection.GetServer("127.0.0.1" + ":" + "6379").Keys();
                count = keys.Where(x => x.ToString().Contains(forderName)).Select(key => key).Count();
            }
            catch (Exception exception)
            {
            }
            return count;
        }
    }

}