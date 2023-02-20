using LoggerService.Interfaces;
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

        public async Task LogCCu(DateTime time, long ccu)
        {
            var database = _client.GetDatabase(NO_SQL_CONFIG.API_TRACKING_SYSTEM_DATABASE_NAME);
            try
            {
                var document = new BsonDocument {
                { "ActionTime", DateTime.SpecifyKind(time, DateTimeKind.Local) },
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
    }
}
