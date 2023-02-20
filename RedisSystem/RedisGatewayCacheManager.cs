using LoggerService;
using RedisSystem.Config;
using ShareData.LogSystem;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisSystem
{
    public class RedisGatewayCacheManager
    {
        private static ILoggerManager _logger;
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
                        {
                            _inst = new RedisGatewayCacheManager();
                            _logger = new LoggerManager();
                        }
                    }
                }
                return _inst;
            }
        }
        private static string linkConnect = RedisConfig.RedisServerIpAddress + ":" + RedisConfig.RedisServerPort + ",password=" + RedisConfig.Password + ",abortConnect=false";
        // Redis Connection string info
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            string cacheConnection = linkConnect;
            return ConnectionMultiplexer.Connect(cacheConnection);
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }

        public async Task<long> CountItemByString(string forderName)
        {
            long count = 0;
            try
            {
                var server = Connection.GetServer(RedisConfig.RedisServerIpAddress + ":" + RedisConfig.RedisServerPort);
                var db = Connection.GetDatabase();
                count = server.Keys(0, forderName + ":*", pageSize: 500).Count();
            }
            catch (Exception exception)
            {
            }
            return count;
        }


        public async Task DeleteArrayKeyAsync(string forderName)
        {
            try
            {
                //var keys = Connection.GetServer(RedisConfig.RedisServerIpAddress + ":" + RedisConfig.RedisServerPort).Keys();
                //RedisKey[] keysArr = keys.Where(x => x.ToString().Contains(forderName)).Select(key => key).ToArray();
                //await Connection.GetDatabase().KeyDeleteAsync(keysArr).ConfigureAwait(false);
                var server = Connection.GetServer(RedisConfig.RedisServerIpAddress + ":" + RedisConfig.RedisServerPort);
                var db = Connection.GetDatabase();
                foreach (var key in server.Keys(0, forderName+":*",  pageSize: 500))
                {
                    await db.KeyDeleteAsync(key, flags: CommandFlags.FireAndForget).ConfigureAwait(false);
                }
            }
            catch (Exception exception)
            {
            }
        }

        public async Task<bool> SaveDataAsync(string key, string value, long minuteExpires = -1)
        {
            try
            {
                var _database = Connection.GetDatabase();
                if (minuteExpires < 0)
                    return _database.StringSet(key, value);
                var timeExpires = TimeSpan.FromMinutes(minuteExpires);
                return await _database.StringSetAsync(key, value, timeExpires);
            }
            catch (Exception exception)
            {
                await _logger.LogError("Redis-SaveData()", exception.ToString()).ConfigureAwait(false);
                return false;
            }
        }
        public bool SaveDataSecond(string key, string value, long secondExpires = -1)
        {
            try
            {
                var _database = Connection.GetDatabase();
                if (secondExpires < 0)
                    return _database.StringSet(key, value);
                var timeExpires = TimeSpan.FromSeconds(secondExpires);
                return _database.StringSet(key, value, timeExpires);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("Redis-SaveData()", exception.ToString()).ConfigureAwait(false));
                return false;
            }
        }

        public async Task<string> GetDataFromCacheAsync(string key)
        {
            try
            {
                var _database = Connection.GetDatabase();
                return await _database.StringGetAsync(key);
            }
            catch (Exception exception)
            {
                await _logger.LogError("Redis-GetDataFromCache()", exception.ToString()).ConfigureAwait(false);
                return "";
            }
        }

        public async Task DeleteDataFromCacheAsync(string key)
        {
            try
            {
                var _database = Connection.GetDatabase();
                await _database.KeyDeleteAsync(key);
            }
            catch (Exception exception)
            {
                await _logger.LogError("Redis-DeleteDataFromCache()", exception.ToString()).ConfigureAwait(false);
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
                Task.Run(async () => await _logger.LogError("Redis-CheckExistKey()", exception.ToString()).ConfigureAwait(false));
                return false;
            }
        }

        public void DeleteArrayKey(string forderName)
        {
            try
            {
                var keys = Connection.GetServer(RedisConfig.RedisServerIpAddress + ":" + RedisConfig.RedisServerPort).Keys();
                RedisKey[] keysArr = keys.Where(x => x.ToString().Contains(forderName)).Select(key => key).ToArray();
                Connection.GetDatabase().KeyDelete(keysArr);
            }
            catch (Exception exception)
            {
            }
        }
        public void DeleteKeyForder(string forderName, string name)
        {
            try
            {
                var keys = Connection.GetServer(RedisConfig.RedisServerIpAddress + ":" + RedisConfig.RedisServerPort).Keys();
                RedisKey[] keysArr = keys.Where(x => x.ToString().Contains(forderName) && x.ToString().Contains(name)).Select(key => key).ToArray();
                Connection.GetDatabase().KeyDelete(keysArr);
            }
            catch (Exception exception)
            {
            }
        }
    }
}
