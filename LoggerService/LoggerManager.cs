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

        public List<LogMongoModel> GetLog(int Type, int Page, int PageRow) {

            if (PageRow > 500)
                PageRow = 500;
            var database = _client.GetDatabase(NO_SQL_CONFIG.API_LOG_SYSTEM_DATABASE_NAME);
            IMongoCollection<LogMongoModel> trackingCollection = database.GetCollection<LogMongoModel>(NO_SQL_CONFIG.API_LOG_NORMAL_COLLECTION);
            if (Type == 1) {
                trackingCollection = database.GetCollection<LogMongoModel>(NO_SQL_CONFIG.API_LOG_ERROR_COLLECTION);
            }
            List<LogMongoModel> model = new List<LogMongoModel>();
            try
            {
                model = trackingCollection.AsQueryable<LogMongoModel>().Where(x => x.Id != null).OrderByDescending(y => y.ActionTime).Skip(PageRow * Page).Take(PageRow).ToList();
            }
            catch (Exception ex)
            {
                LogError("GetLog", ex.ToString()).ConfigureAwait(false);
            }
            return model;
        }

        public void Dispose()
        {
            _client.Cluster.Dispose();
        }
    }

}
