using DAO.DAOImp;
using RedisSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerEventTet2023.Instance
{
    public class ConfigRedisInstance
    {
        private static ConfigRedisInstance _inst;
        private static object _syncObject = new object();

        public static ConfigRedisInstance Inst
        {
            get
            {
                if (_inst == null)
                    lock (_syncObject)
                    {
                        if (_inst == null) _inst = new ConfigRedisInstance();
                    }
                return _inst;
            }
        }

    }
}
