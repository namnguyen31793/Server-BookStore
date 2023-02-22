using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace JobWindowsService.Mongo
{
    public class LogSystemInstance
    {
        private static object _syncObject = new object();
        private static LogSystemInstance _inst { get; set; }
        private static MongoClient _client;
        public static LogSystemInstance Inst
        {
            get
            {
                if (_inst == null)
                    lock (_syncObject)
                    {
                        if (_inst == null) _inst = new LogSystemInstance();
                        MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(NO_SQL_CONFIG.HOST_CONFIG));
                        settings.SslSettings = new SslSettings() { EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 };
                        settings.MaxConnectionIdleTime = TimeSpan.FromSeconds(30);
                        _client = new MongoClient(settings);
                    }
                return _inst;
            }
        }

        public void Tracking_Ccu_Log(long ccu)
        {
            try
            {
                var database = _client.GetDatabase(NO_SQL_CONFIG.API_TRACKING_SYSTEM_DATABASE_NAME);
                IMongoCollection<BsonDocument> trackingCollection = database.GetCollection<BsonDocument>(NO_SQL_CONFIG.API_LOG_CCU_COLLECTION);

                var key = Builders<BsonDocument>.IndexKeys.Ascending("ExpireAt");
                var optionsIndex = new CreateIndexOptions
                {
                    ExpireAfter = new TimeSpan(0),
                    Name = "ExpireAtIndex",

                };
                trackingCollection.Indexes.CreateOneAsync(key, optionsIndex).ConfigureAwait(false);

                    BsonDocument document = new BsonDocument { { "Ccu", ccu }, { "ActionTime", DateTime.Now }, { "ExpireAt", DateTime.SpecifyKind(DateTime.Now.AddDays(15), DateTimeKind.Local) } };
                    trackingCollection.InsertOneAsync(document);
                
            }
            catch (Exception exception)
            {
            }
        }
    }
}
