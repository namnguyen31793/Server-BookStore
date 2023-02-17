using LoggerService.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerService
{
    public class LoggerManager : ILoggerManager, IDisposable
    {

        private readonly MongoClient _client;

        public LoggerManager()
        {
            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(NO_SQL_CONFIG.HOST_CONFIG));
            settings.SslSettings = new SslSettings() { EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 };
            settings.MaxConnectionIdleTime = TimeSpan.FromSeconds(30);
            //_client = new MongoClient(NO_SQL_CONFIG.HOST_CONFIG);
            _client = new MongoClient(settings);
        }


        public async Task LogDebug(string title, string message) {
            var document = new BsonDocument {
                { "Title", title },
                { "Content", message },
                { "ExpireAt", DateTime.SpecifyKind(DateTime.Now.AddDays(15), DateTimeKind.Local) },
                { "ActionTime", DateTime.Now } };
            var database = _client.GetDatabase(NO_SQL_CONFIG.API_LOG_SYSTEM_DATABASE_NAME); 
            IMongoCollection<BsonDocument> trackingCollection = database.GetCollection<BsonDocument>(NO_SQL_CONFIG.API_LOG_DEBUG_COLLECTION);
            
            try
            {
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

        public async Task LogError(string title, string message)
        {
            var document = new BsonDocument {
                { "Title", title },
                { "Content", message },
                { "Status", "" },
                { "ExpireAt", DateTime.SpecifyKind(DateTime.Now.AddDays(15), DateTimeKind.Local) },
                { "ActionTime", DateTime.Now } };
            var database = _client.GetDatabase(NO_SQL_CONFIG.API_LOG_SYSTEM_DATABASE_NAME);
            IMongoCollection<BsonDocument> trackingCollection = database.GetCollection<BsonDocument>(NO_SQL_CONFIG.API_LOG_ERROR_COLLECTION);

            try
            {
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

        public async Task LogInfo(string title, string message, string status)
        {
            var document = new BsonDocument {
                { "Title", title },
                { "Content", message },
                { "Status", status },
                { "ExpireAt", DateTime.SpecifyKind(DateTime.Now.AddDays(15), DateTimeKind.Local) },
                { "ActionTime", DateTime.Now } };
            var database = _client.GetDatabase(NO_SQL_CONFIG.API_LOG_SYSTEM_DATABASE_NAME);
            IMongoCollection<BsonDocument> trackingCollection = database.GetCollection<BsonDocument>(NO_SQL_CONFIG.API_LOG_NORMAL_COLLECTION);

            try
            {
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

        public Task LogWarn(string message)
        {
            throw new NotImplementedException();
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

        public void Dispose()
        {
            _client.Cluster.Dispose();
        }
    }

}
