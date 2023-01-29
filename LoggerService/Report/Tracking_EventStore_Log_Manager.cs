using LoggerService.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerService.Report
{
    public class Tracking_EventStore_Log_Manager : IDisposable
    {
        private static object _syncObject = new object();
        private static Tracking_EventStore_Log_Manager _inst { get; set; }
        public static Tracking_EventStore_Log_Manager Inst
        {
            get
            {
                if (_inst == null)
                    lock (_syncObject)
                    {
                        if (_inst == null) _inst = new Tracking_EventStore_Log_Manager();

                        MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(NO_SQL_CONFIG.HOST_CONFIG));
                        settings.SslSettings = new SslSettings() { EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 };
                        settings.MaxConnectionIdleTime = TimeSpan.FromSeconds(30);
                        _client = new MongoClient(settings);
                    }
                return _inst;
            }
        }
        private static MongoClient _client;

        private Tracking_EventStore_Log_Manager()
        {

        }

        public void Dispose()
        {
            _client.Cluster.Dispose();
        }

    }
}
