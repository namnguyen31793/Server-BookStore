using LoggerService.Interfaces;
using LoggerService.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using ShareData.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerService
{
    public class ReportManager : IReportLog, IDisposable
    {

        private readonly MongoClient _client;

        public ReportManager()
        {
            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(NO_SQL_CONFIG.HOST_CONFIG));
            settings.SslSettings = new SslSettings() { EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 };
            settings.MaxConnectionIdleTime = TimeSpan.FromSeconds(30);
            //_client = new MongoClient(NO_SQL_CONFIG.HOST_CONFIG);
            _client = new MongoClient(settings);
        }

        public void Dispose()
        {
        }

        public async Task LogBuyBook(string title, long AccountId, string barcode, long price)
        {
            var database = _client.GetDatabase(NO_SQL_CONFIG.API_TRACKING_SYSTEM_DATABASE_NAME);
            try
            {
                var document = new BsonDocument {
                { "Title", title },
                { "AccountId", AccountId },
                { "Barcode", barcode },
                { "Price", price },
                { "ExpireAt", DateTime.SpecifyKind(DateTime.Now.AddMonths(6), DateTimeKind.Local) },
                { "ActionTime", DateTime.Now } };
                IMongoCollection<BsonDocument> trackingCollection = database.GetCollection<BsonDocument>(NO_SQL_CONFIG.API_LOG_BUY_BOOK_COLLECTION);

                var indexBuilder = Builders<BsonDocument>.IndexKeys;
                var key = indexBuilder.Ascending("ExpireAt");
                var options = new CreateIndexOptions
                {
                    ExpireAfter = new TimeSpan(0),
                    Name = "ExpireAtIndex",

                };
                await trackingCollection.Indexes.CreateOneAsync(key, options).ConfigureAwait(false);

                await trackingCollection.InsertOneAsync(document);
            }
            catch (Exception)
            {
            }
            finally
            {
                database = null;
            }
        }

        public async Task LogCCu(DateTime time, long ccu)
        {
            var database = _client.GetDatabase(NO_SQL_CONFIG.API_TRACKING_SYSTEM_DATABASE_NAME);
            try
            {
                var document = new BsonDocument {
                { "Ccu", ccu },
                { "ExpireAt", DateTime.SpecifyKind(DateTime.Now.AddDays(15), DateTimeKind.Local) },
                { "ActionTime", DateTime.Now } };
                IMongoCollection<BsonDocument> trackingCollection = database.GetCollection<BsonDocument>(NO_SQL_CONFIG.API_LOG_CCU_COLLECTION);

                var indexBuilder = Builders<BsonDocument>.IndexKeys;
                var key = indexBuilder.Ascending("ExpireAt");
                var options = new CreateIndexOptions
                {
                    ExpireAfter = new TimeSpan(0),
                    Name = "ExpireAtIndex",

                };
                await trackingCollection.Indexes.CreateOneAsync(key, options).ConfigureAwait(false);

                await trackingCollection.InsertOneAsync(document);
            }
            catch (Exception)
            {
            }
            finally
            {
                database = null;
            }
        }

        public async Task TrackingActionHome(string data)
        {
            if (string.IsNullOrEmpty(data))
                return;
            List<RequestTrackingAction2> listAction = JsonConvert.DeserializeObject<List<RequestTrackingAction2>>(data);
            if(listAction == null || listAction.Count == 0)
                return;
            var database = _client.GetDatabase(NO_SQL_CONFIG.API_TRACKING_SYSTEM_DATABASE_NAME);
            try
            {
                var listDocument = new List<BsonDocument>();
                for (int i = 0; i < listAction.Count; i++) {
                    listDocument.Add(new BsonDocument {
                        { "AccountId", listAction[i].AccountId },
                        { "Action", listAction[i].Action },
                        { "Deep", listAction[i].Deep },
                        { "Count", listAction[i].Count },
                        { "ActionTime", DateTime.Now.ToString("MM/dd/yyyy") } ,
                        { "ExpireAt", DateTime.SpecifyKind(DateTime.Now.AddMonths(2), DateTimeKind.Local) }});
                }
                IMongoCollection<BsonDocument> trackingCollection = database.GetCollection<BsonDocument>(NO_SQL_CONFIG.API_LOG_TRACKING_ACTION_HOME);

                var indexBuilder = Builders<BsonDocument>.IndexKeys;
                var key = indexBuilder.Ascending("ExpireAt");
                var options = new CreateIndexOptions
                {
                    ExpireAfter = new TimeSpan(0),
                    Name = "ExpireAtIndex",

                };
                await trackingCollection.Indexes.CreateOneAsync(key, options).ConfigureAwait(false);

                await trackingCollection.InsertManyAsync(listDocument);
            }
            catch (Exception)
            {
            }
            finally
            {
                database = null;
            }
        }

        public async Task TrackingActionUser(string data)
        {
            if (string.IsNullOrEmpty(data))
                return;
            List<RequestTrackingAction> listAction = JsonConvert.DeserializeObject<List<RequestTrackingAction>>(data);
            if (listAction == null || listAction.Count == 0)
                return;
            var database = _client.GetDatabase(NO_SQL_CONFIG.API_TRACKING_SYSTEM_DATABASE_NAME);
            try
            {
                var listDocument = new List<BsonDocument>();
                for (int i = 0; i < listAction.Count; i++)
                {
                    listDocument.Add(new BsonDocument {
                        { "AccountId", listAction[i].AccountId },
                        { "Action", listAction[i].Action },
                        { "Count", listAction[i].Count },
                        { "ActionTime", DateTime.Now.ToString("MM/dd/yyyy") },
                        { "ExpireAt", DateTime.SpecifyKind(DateTime.Now.AddMonths(2), DateTimeKind.Local) }});
                }
                IMongoCollection<BsonDocument> trackingCollection = database.GetCollection<BsonDocument>(NO_SQL_CONFIG.API_LOG_TRACKING_ACTION_USER);

                var indexBuilder = Builders<BsonDocument>.IndexKeys;
                var key = indexBuilder.Ascending("ExpireAt");
                var options = new CreateIndexOptions
                {
                    ExpireAfter = new TimeSpan(0),
                    Name = "ExpireAtIndex",

                };
                await trackingCollection.Indexes.CreateOneAsync(key, options).ConfigureAwait(false);

                await trackingCollection.InsertManyAsync(listDocument);
            }
            catch (Exception)
            {
            }
            finally
            {
                database = null;
            }
        }

        public async Task TrackingFindBook(string data)
        {
            if (string.IsNullOrEmpty(data))
                return;
            List<RequestTrackingFindBook> listAction = JsonConvert.DeserializeObject<List<RequestTrackingFindBook>>(data);
            if (listAction == null || listAction.Count == 0)
                return;
            var database = _client.GetDatabase(NO_SQL_CONFIG.API_TRACKING_SYSTEM_DATABASE_NAME);
            try
            {
                var listDocument = new List<BsonDocument>();
                for (int i = 0; i < listAction.Count; i++)
                {
                    listDocument.Add(new BsonDocument {
                        { "AccountId", listAction[i].AccountId },
                        { "Action", listAction[i].Action },
                        { "Barcode", listAction[i].Barcode },
                        { "Count", listAction[i].Count },
                        { "ActionTime", DateTime.Now.ToString("MM/dd/yyyy") },
                        { "ExpireAt", DateTime.SpecifyKind(DateTime.Now.AddMonths(2), DateTimeKind.Local) }});
                }
                IMongoCollection<BsonDocument> trackingCollection = database.GetCollection<BsonDocument>(NO_SQL_CONFIG.API_LOG_TRACKING_FIND_BOOK);

                var indexBuilder = Builders<BsonDocument>.IndexKeys;
                var key = indexBuilder.Ascending("ExpireAt");
                var options = new CreateIndexOptions
                {
                    ExpireAfter = new TimeSpan(0),
                    Name = "ExpireAtIndex",

                };
                await trackingCollection.Indexes.CreateOneAsync(key, options).ConfigureAwait(false);

                await trackingCollection.InsertManyAsync(listDocument);
            }
            catch (Exception)
            {
            }
            finally
            {
                database = null;
            }
        }

        public async Task TrackingListenAudio(string data)
        {
            if (string.IsNullOrEmpty(data))
                return;
            List<RequestTrackingListenAudio> listAction = JsonConvert.DeserializeObject<List<RequestTrackingListenAudio>>(data);
            if (listAction == null || listAction.Count == 0)
                return;
            var database = _client.GetDatabase(NO_SQL_CONFIG.API_TRACKING_SYSTEM_DATABASE_NAME);
            try
            {
                var listDocument = new List<BsonDocument>();
                for (int i = 0; i < listAction.Count; i++)
                {
                    listDocument.Add(new BsonDocument {
                        { "AccountId", listAction[i].AccountId },
                        { "Barcode", listAction[i].Barcode },
                        { "Count", listAction[i].Count },
                        { "ActionTime", DateTime.Now.ToString("MM/dd/yyyy") },
                        { "ExpireAt", DateTime.SpecifyKind(DateTime.Now.AddMonths(2), DateTimeKind.Local) }});
                }
                IMongoCollection<BsonDocument> trackingCollection = database.GetCollection<BsonDocument>(NO_SQL_CONFIG.API_LOG_TRACKING_LISTEN_AUDIO);

                var indexBuilder = Builders<BsonDocument>.IndexKeys;
                var key = indexBuilder.Ascending("ExpireAt");
                var options = new CreateIndexOptions
                {
                    ExpireAfter = new TimeSpan(0),
                    Name = "ExpireAtIndex",

                };
                await trackingCollection.Indexes.CreateOneAsync(key, options).ConfigureAwait(false);

                await trackingCollection.InsertManyAsync(listDocument);
            }
            catch (Exception)
            {
            }
            finally
            {
                database = null;
            }
        }
    }
}
