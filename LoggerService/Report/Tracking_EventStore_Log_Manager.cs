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

        
        #region LOG SPIN
        public async Task Tracking_EventStore_Log_Buy_GiftCode(long AccountID, long TransId)
        {

            var database = _client.GetDatabase(NO_SQL_CONFIG.T09_SYSTEM_DATABASE_NAME);
            string collectionName = NO_SQL_CONFIG.T09_EVENT_STORE_BUY_GIFTCODE_LOG_COLLECTION;

            IMongoCollection<BsonDocument> trackingCollection = database.GetCollection<BsonDocument>(collectionName);

            var indexBuilder = Builders<BsonDocument>.IndexKeys;
            var key = indexBuilder.Ascending("ExpireAt");
            var options = new CreateIndexOptions
            {
                ExpireAfter = new TimeSpan(0),
                Name = "ExpireAtIndex",

            };
            await trackingCollection.Indexes.CreateOneAsync(key, options).ConfigureAwait(false);

            BsonDocument filter = new BsonDocument();
            filter.Add("AccountID", new BsonInt64(AccountID));
            var accountDocument = trackingCollection.Find(filter).FirstOrDefault();
            List<long> listId = new List<long>() { TransId};
            if (accountDocument == null || accountDocument == BsonNull.Value)
            {
                BsonDocument document = new BsonDocument {
                { "AccountID", AccountID },
                { "TransIds", JsonConvert.SerializeObject(listId) },
                { "ActionTime", DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local) },
                { "ExpireAt", DateTime.SpecifyKind(DateTime.Now.AddDays(120), DateTimeKind.Local) }};

                await trackingCollection.InsertOneAsync(document).ConfigureAwait(false);
            } else
            {
                var options2 = new FindOneAndUpdateOptions<BsonDocument>
                {
                    ReturnDocument = ReturnDocument.After
                };
                var model = BsonSerializer.Deserialize<Tracking_Event_Store_Buy_GiftCode_Log_Model>(accountDocument.ToBsonDocument());
                List<long> listTrans = JsonConvert.DeserializeObject<List<long>>(model.TransIds);
                listTrans.Add(TransId);
                await trackingCollection.FindOneAndUpdateAsync(filter,
                               Builders<BsonDocument>.Update
                               .Set("TransIds", JsonConvert.SerializeObject(listTrans))
                               .Set("ActionTime", DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local))
                               , options2);
            }
        }
        public async Task Tracking_EventStore_Log_Buy_GiftCode(long AccountID, string ProductId)
        {

            var database = _client.GetDatabase(NO_SQL_CONFIG.T09_SYSTEM_DATABASE_NAME);
            string collectionName = NO_SQL_CONFIG.T09_EVENT_STORE_BUY_GIFTCODE_LOG_COLLECTION;

            IMongoCollection<BsonDocument> trackingCollection = database.GetCollection<BsonDocument>(collectionName);

            var indexBuilder = Builders<BsonDocument>.IndexKeys;
            var key = indexBuilder.Ascending("ExpireAt");
            var options = new CreateIndexOptions
            {
                ExpireAfter = new TimeSpan(0),
                Name = "ExpireAtIndex",

            };
            await trackingCollection.Indexes.CreateOneAsync(key, options).ConfigureAwait(false);

            BsonDocument filter = new BsonDocument();
            filter.Add("AccountID", new BsonInt64(AccountID));
            var accountDocument = trackingCollection.Find(filter).FirstOrDefault();
            List<string> listId = new List<string>() { ProductId };
            if (accountDocument == null || accountDocument == BsonNull.Value)
            {
                BsonDocument document = new BsonDocument {
                { "AccountID", AccountID },
                { "TransIds", JsonConvert.SerializeObject(listId) },
                { "ActionTime", DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local) },
                { "ExpireAt", DateTime.SpecifyKind(DateTime.Now.AddDays(120), DateTimeKind.Local) }};

                await trackingCollection.InsertOneAsync(document).ConfigureAwait(false);
            }
            else
            {
                var options2 = new FindOneAndUpdateOptions<BsonDocument>
                {
                    ReturnDocument = ReturnDocument.After
                };
                var model = BsonSerializer.Deserialize<Tracking_Event_Store_Buy_GiftCode_Log_Model>(accountDocument.ToBsonDocument());
                List<string> listTrans = JsonConvert.DeserializeObject<List<string>>(model.TransIds);
                listTrans.Add(ProductId);
                await trackingCollection.FindOneAndUpdateAsync(filter,
                               Builders<BsonDocument>.Update
                               .Set("TransIds", JsonConvert.SerializeObject(listTrans))
                               .Set("ActionTime", DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local))
                               , options2);
            }
        }

        public Tracking_Event_Store_Buy_GiftCode_Log_Model Get_Tracking_EventStore_Log_Buy_GiftCode(long accountId = 0)
        {
            if (accountId == 0)
                return null;
            try
            {
                var database = _client.GetDatabase(NO_SQL_CONFIG.T09_SYSTEM_DATABASE_NAME);
                IMongoCollection<Tracking_Event_Store_Buy_GiftCode_Log_Model> trackingCollection = database.GetCollection<Tracking_Event_Store_Buy_GiftCode_Log_Model>(NO_SQL_CONFIG.T09_EVENT_STORE_BUY_GIFTCODE_LOG_COLLECTION);

                Tracking_Event_Store_Buy_GiftCode_Log_Model model = trackingCollection.AsQueryable<Tracking_Event_Store_Buy_GiftCode_Log_Model>().FirstOrDefault(x => x.AccountID == accountId);
                
                return model;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void Dispose()
        {
            _client.Cluster.Dispose();
        }
        #endregion

    }
}
