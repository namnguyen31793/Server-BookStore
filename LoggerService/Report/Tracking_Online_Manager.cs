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
    public class Tracking_Online_Manager
    {
        private static object _syncObject = new object();
        private static Tracking_Online_Manager _inst { get; set; }
        public static Tracking_Online_Manager Inst
        {
            get
            {
                if (_inst == null)
                    lock (_syncObject)
                    {
                        if (_inst == null)
                        {
                            _inst = new Tracking_Online_Manager();

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

        public async Task<List<Log_Oniline_Model>> GetListOnlineByTime(DateTime start, DateTime end)
        {
            List<Log_Oniline_Model> listDataBson = new List<Log_Oniline_Model>();
            var database = _client.GetDatabase(NO_SQL_CONFIG.API_TRACKING_SYSTEM_DATABASE_NAME);
            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(NO_SQL_CONFIG.API_LOG_TRACKING_ONLINE);
            var pipeline = new[]{new BsonDocument(){
                {"$match", new BsonDocument(){
                    {"StartActionTime", new BsonDocument(){
                        {"$gte", start},
                        {"$lt", end}
                    }}
                }}
            },new BsonDocument(){
                {"$group", new BsonDocument(){
                    {"_id", new BsonDocument(){
                        {"AccountId", "$AccountId"}
                    }},
                    {"TimePlay", new BsonDocument(){
                        {"$sum", "$Time"}
                    }}
                }}
            },new BsonDocument(){
                {"$project", new BsonDocument(){
                    {"TimePlay", "$TimePlay"},
                    {"AccountId", "$_id.AccountId"}
                }}
            },new BsonDocument(){
                {"$sort", new BsonDocument(){
                    {"_id", -1}
                }}
            }};

            var results = await collection.Aggregate<BsonDocument>(pipeline).ToListAsync();
            foreach (var document in results)
            {
                var databson = BsonSerializer.Deserialize<Log_Oniline_Model>(document.ToBsonDocument());
                if (databson != null)
                    listDataBson.Add(databson);
            }
            return listDataBson;
        }
    }
}
