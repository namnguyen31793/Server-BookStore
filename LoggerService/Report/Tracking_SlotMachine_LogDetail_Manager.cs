using LoggerService.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerService.Report
{
    public class Tracking_SlotMachine_LogDetail_Manager : IDisposable
    {
        private static object _syncObject = new object();
        private static Tracking_SlotMachine_LogDetail_Manager _inst { get; set; }
        public static Tracking_SlotMachine_LogDetail_Manager Inst
        {
            get
            {
                if (_inst == null)
                    lock (_syncObject)
                    {
                        if (_inst == null)
                        {
                            _inst = new Tracking_SlotMachine_LogDetail_Manager();

                            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(NO_SQL_CONFIG.HOST_CONFIG));
                            settings.SslSettings = new SslSettings() { EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 };
                            settings.MaxConnectionIdleTime = TimeSpan.FromSeconds(30);
                            //_client = new MongoClient(NO_SQL_CONFIG.HOST_CONFIG);
                            _client = new MongoClient(settings);
                        }
                    }
                return _inst;
            }
        }
        private static MongoClient _client;

        private Tracking_SlotMachine_LogDetail_Manager()
        {

        }

        public static string GetToDay_Totring(DateTime dateTime)
        {
            string tempDayString = String.Format("{0:d_M_yyyy}", dateTime);
            return tempDayString;
        }

        public void Dispose()
        {
            _client.Cluster.Dispose();
        }
    }
}
