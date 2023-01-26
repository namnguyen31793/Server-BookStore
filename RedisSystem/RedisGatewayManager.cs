using LoggerService;
using Newtonsoft.Json;
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
    public class RedisGatewayManager<T>
    {
        private static ILoggerManager _logger;
        private static readonly object SyncObject = new object();

        private static RedisGatewayManager<T> _inst;

        public static RedisGatewayManager<T> Inst
        {
            get
            {
                if (_inst == null)
                {
                    lock (SyncObject)
                    {
                        if (_inst == null)
                        {
                            _inst = new RedisGatewayManager<T>();
                            _logger = new LoggerManager();
                        }
                    }
                }
                return _inst;
            }
        }

        // Redis Connection string info
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            string cacheConnection = RedisConfig.RedisServerIpAddress + ":" + RedisConfig.RedisServerPort + ",password=" + RedisConfig.Password + ",abortConnect=false";
            return ConnectionMultiplexer.Connect(cacheConnection);
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }

        public bool SaveByte(string key, byte[] value, long seconds = -1)
        {
            try
            {
                TimeSpan? timeExpires = null;
                if (seconds > 0)
                    timeExpires = TimeSpan.FromSeconds(seconds);
                return Connection.GetDatabase().StringSet(key, value, timeExpires);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("Redis-SaveByte()", exception.ToString()).ConfigureAwait(false));
                return false;
            }
        }

        public bool SaveData(string key, string value, long seconds = -1)
        {
            try
            {
                TimeSpan? timeExpires = null;
                if (seconds > 0)
                    timeExpires = TimeSpan.FromSeconds(seconds);
                return Connection.GetDatabase().StringSet(key, value, timeExpires);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("Redis-SaveData()", exception.ToString()).ConfigureAwait(false));
                return false;
            }
        }

        public string GetDataFromCache(string key)
        {
            return Connection.GetDatabase().StringGet(key);
        }

        public List<T> GetArrayKey(string forderName)
        {
            try
            {
                var keys = Connection.GetServer(RedisConfig.RedisServerIpAddress + ":" + RedisConfig.RedisServerPort).Keys();
                RedisKey[] keysArr = keys.Where(x => x.ToString().Contains(forderName)).Select(key => key).ToArray();
                RedisValue[] queueValues = Connection.GetDatabase().StringGet(keysArr);
                List<T> queues = queueValues.Select(qv => JsonConvert.DeserializeObject<T>(qv)).ToList();
                return queues;
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("Redis-GetArrayKey()", exception.ToString()).ConfigureAwait(false));
                return null;
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

        public async Task<TResult> GetFromCache<TResult>(
            string key,
            string val,
            int seconds,
            Func<Task<T>> func)
        {
            var cacheKey = key;
            if (!string.IsNullOrEmpty(val))
                cacheKey += ":" + val;
            var data = Connection.GetDatabase().StringGet(cacheKey);

            if (string.IsNullOrEmpty(data))
            {
                data = JsonConvert.SerializeObject(await func());
                TimeSpan? timeExpires = null;
                if (seconds > 0)
                    timeExpires = TimeSpan.FromSeconds(seconds);
                await Connection.GetDatabase().StringSetAsync(
                    cacheKey,
                    data,
                    timeExpires);
            }

            return JsonConvert.DeserializeObject<TResult>(data);
        }
    }
}
