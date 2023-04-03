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

        public async Task<List<Log_Oniline_Hours_Model>> GetListOnlineHourseByTime(DateTime start, DateTime end)
        {
            List<Log_Oniline_Hours_Model> listDataBson = new List<Log_Oniline_Hours_Model>();

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
                        {"TimePlay", new BsonDocument(){
                            {"$dateToString", new BsonDocument(){
                                {"format", "%Y-%m-%d %H"},
                                {"date", "$StartActionTime"}
                            }}
                        }}
                    }},
                    {"TimeOnline", new BsonDocument(){
                        {"$sum", "$Time"}
                    }}
                }}
            },new BsonDocument(){
                {"$project", new BsonDocument(){
                    {"_id", "$_id"},
                    {"TimeOnline", "$TimeOnline"},
                    {"TimePlay", "$_id.TimePlay"}
                }}
            }};

            var results = await collection.Aggregate<BsonDocument>(pipeline).ToListAsync();
            foreach (var document in results)
            {
                var databson = BsonSerializer.Deserialize<Log_Oniline_Hours_Model>(document.ToBsonDocument());
                if (databson != null)
                    listDataBson.Add(databson);
            }
            return listDataBson;
        }

        public async Task AddLogHoursOnline(string keySave, long timeOnline)
        {
            var database = _client.GetDatabase(NO_SQL_CONFIG.API_TRACKING_SYSTEM_DATABASE_NAME);
            IMongoCollection<BsonDocument> trackingCollectionFind = database.GetCollection<BsonDocument>(NO_SQL_CONFIG.API_LOG_TRACKING_ONLINE_BY_HOURS);

            BsonDocument filter = new BsonDocument();
            filter.Add("Hours", keySave);

            BsonDocument sort = new BsonDocument();
            sort.Add("ExpireAt", -1.0);
            var options = new FindOptions<BsonDocument>()
            {
                Sort = sort,
                Limit = 1
            };
            var resultValue = trackingCollectionFind.FindSync(filter, options).FirstOrDefault();
            if (resultValue == null)
            {
                var document = new BsonDocument {
                    { "Hours", keySave },
                    { "Time", timeOnline },
                    { "ExpireAt", DateTime.SpecifyKind(DateTime.Now.AddMonths(3), DateTimeKind.Local) } };
                var indexBuilder = Builders<BsonDocument>.IndexKeys;
                var key = indexBuilder.Ascending("ExpireAt");
                var optionsNew = new CreateIndexOptions
                {
                    ExpireAfter = new TimeSpan(0),
                    Name = "ExpireAtIndex",

                };
                await trackingCollectionFind.Indexes.CreateOneAsync(key, optionsNew).ConfigureAwait(false);

                await trackingCollectionFind.InsertOneAsync(document).ConfigureAwait(false);
                    
            }
        }
    }
}
