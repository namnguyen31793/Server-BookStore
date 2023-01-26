using LoggerService.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using ShareData.Game.Slot;
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

        #region LOG SPIN
        public async Task Tracking_SlotMachine_Detail_Insert_SpinData(int GameID, long SpinID, int RoomID, long AccountID, string Username, string Nickname, string Matrix, long TotalNormalReward, long Total, long TotalSpecialReward, long AccountBalance, string AccountBagInfo, string Ip, bool isJackpot = false, string ExtendMatrixDescription = "0")
        {

            var client = new MongoClient(NO_SQL_CONFIG.HOST_CONFIG);
            var database = client.GetDatabase(NO_SQL_CONFIG.T09_SYSTEM_DATABASE_NAME);

            string collectionName = NO_SQL_CONFIG.Get_Slot_DetailSpin_CollectionName(GameID);
            if (string.IsNullOrEmpty(NO_SQL_CONFIG.Get_Slot_DetailSpin_CollectionName(GameID)))
                return;
            IMongoCollection<BsonDocument> trackingCollection = database.GetCollection<BsonDocument>(collectionName);

            var indexBuilder = Builders<BsonDocument>.IndexKeys;
            var key = indexBuilder.Ascending("ExpireAt");
            var options = new CreateIndexOptions
            {
                ExpireAfter = new TimeSpan(0),
                Name = "ExpireAtIndex",

            };
            await trackingCollection.Indexes.CreateOneAsync(key, options).ConfigureAwait(false);

            BsonDocument document = new BsonDocument {
                { "SpinID", SpinID },
                { "Bet", RoomID },
                { "AccountID",AccountID },
                { "Username", Username },
                { "Nickname", Nickname },
                { "GameID", GameID },
                { "Matrix", Matrix },
                { "TotalNormalReward", TotalNormalReward },
                { "Total", Total },
                { "TotalSpecialReward", TotalSpecialReward },
                { "Ip", Ip },
                { "AccountBalance", AccountBalance },
                { "AccountBagInfo", AccountBagInfo },
                { "Jackpot", isJackpot },
                { "ExtendMatrixDescription", ExtendMatrixDescription },
                { "ActionTime", DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local) },
                { "ExpireAt", DateTime.SpecifyKind(DateTime.Now.AddDays(60), DateTimeKind.Local) }};

            await trackingCollection.InsertOneAsync(document).ConfigureAwait(false);
        }

        public List<Tracking_SlotMachine_Detail_Log_Model> Get_SlotMachine_Detail_SpinData_Log(int GameType, int page = 0, int PageRow = 100, long accountId = 0)
        {
            if (PageRow > 500)
                PageRow = 500;
            List<Tracking_SlotMachine_Detail_Log_Model> models = new List<Tracking_SlotMachine_Detail_Log_Model>();
            try
            {
                var database = _client.GetDatabase(NO_SQL_CONFIG.T09_SYSTEM_DATABASE_NAME);
                string collectionName = NO_SQL_CONFIG.Get_Slot_DetailSpin_CollectionName(GameType);
                if (string.IsNullOrEmpty(collectionName))
                    return models;

                BsonDocument filter = new BsonDocument();
                if (accountId != 0)
                {
                    filter.Add("AccountID", accountId);
                }
                var sort = new BsonDocument(){
                    {"_id", -1}
                };
                var options = new FindOptions<BsonDocument>()
                {
                    Skip = PageRow * page,
                    Sort = sort,
                    Limit = PageRow
                };
                var resultValue = database.GetCollection<BsonDocument>(collectionName).FindSync(filter, options).ToList();
                models = resultValue.Select(y => BsonSerializer.Deserialize<Tracking_SlotMachine_Detail_Log_Model>(y.ToBsonDocument())).ToList(); 
                
                return models;
            }
            catch (Exception)
            {
                return models;
            }
        }
        public List<Tracking_SlotMachine_Detail_Log_Model> Get_SlotMachine_BigWin_SpinData_Log(int GameType)
        {
            List<Tracking_SlotMachine_Detail_Log_Model> models = new List<Tracking_SlotMachine_Detail_Log_Model>();
            try
            {
                var database = _client.GetDatabase(NO_SQL_CONFIG.T09_SYSTEM_DATABASE_NAME);
                string collectionName = NO_SQL_CONFIG.Get_Slot_DetailSpin_CollectionName(GameType);
                if (string.IsNullOrEmpty(NO_SQL_CONFIG.Get_Slot_DetailSpin_CollectionName(GameType)))
                    return models;

                BsonDocument filter = new BsonDocument();
                filter.Add("Total", new BsonDocument("$gte", 300));
                BsonDocument sort = new BsonDocument();
                sort.Add("ActionTime", -1.0);
                var options = new FindOptions<BsonDocument>()
                {
                    Sort = sort,
                    Limit = 50
                };
                var resultValue = database.GetCollection<BsonDocument>(collectionName).FindSync(filter, options).ToList();
                models = resultValue.Select(y => BsonSerializer.Deserialize<Tracking_SlotMachine_Detail_Log_Model>(y.ToBsonDocument())).ToList();

                return models;
            }
            catch (Exception)
            {
                return models;
            }
        }
        #endregion

        #region JACKPOT LOG
        public async Task Tracking_SlotMachine_Detail_Insert_LogJackpot_Fail(int GameID, long SpinID, int RoomID, long AccountID, string Username, string Nickname, string Matrix, long TotalNormalReward, long Total, long TotalSpecialReward, long AccountBalance, string AccountBagInfo, string Ip)
        {
            var database = _client.GetDatabase(NO_SQL_CONFIG.T09_SYSTEM_DATABASE_NAME);

            string collectionName = NO_SQL_CONFIG.Get_Slot_Jackpot_Fail_CollectionName(GameID);
            if (string.IsNullOrEmpty(collectionName))
                return;
            IMongoCollection<BsonDocument> trackingCollection = database.GetCollection<BsonDocument>(collectionName);

            var indexBuilder = Builders<BsonDocument>.IndexKeys;
            var key = indexBuilder.Ascending("ExpireAt");
            var options = new CreateIndexOptions
            {
                ExpireAfter = new TimeSpan(0),
                Name = "ExpireAtIndex",

            };
            await trackingCollection.Indexes.CreateOneAsync(key, options).ConfigureAwait(false);

            BsonDocument document = new BsonDocument {
                { "SpinID", SpinID },
                { "Bet", RoomID },
                { "AccountID",AccountID },
                { "Username", Username },
                { "Nickname", Nickname },
                { "GameID", GameID },
                { "Matrix", Matrix },
                { "TotalNormalReward", TotalNormalReward },
                { "Total", Total },
                { "TotalSpecialReward", TotalSpecialReward },
                { "Ip", Ip },
                { "AccountBalance", AccountBalance },
                { "AccountBagInfo", AccountBagInfo },
                { "ActionTime", DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local) },
                { "ExpireAt", DateTime.SpecifyKind(DateTime.Now.AddDays(60), DateTimeKind.Local) }};

            await trackingCollection.InsertOneAsync(document).ConfigureAwait(false);
        }
        public async Task Tracking_SlotMachine_Detail_Insert_LogJackpot(int GameID, long SpinID, int RoomID, long AccountID, string Username, string Nickname, string Matrix, long TotalNormalReward, long Total, long TotalSpecialReward, long AccountBalance, string AccountBagInfo, string Ip)
        {
            var database = _client.GetDatabase(NO_SQL_CONFIG.T09_SYSTEM_DATABASE_NAME);

            string collectionName = NO_SQL_CONFIG.Get_Slot_Jackpot_Detail_CollectionName(GameID);
            if (string.IsNullOrEmpty(collectionName))
                return;
            IMongoCollection<BsonDocument> trackingCollection = database.GetCollection<BsonDocument>(collectionName);

            var indexBuilder = Builders<BsonDocument>.IndexKeys;
            var key = indexBuilder.Ascending("ExpireAt");
            var options = new CreateIndexOptions
            {
                ExpireAfter = new TimeSpan(0),
                Name = "ExpireAtIndex",

            };
            await trackingCollection.Indexes.CreateOneAsync(key, options).ConfigureAwait(false);

            BsonDocument document = new BsonDocument {
                { "SpinID", SpinID },
                { "Bet", RoomID },
                { "AccountID",AccountID },
                { "Username", Username },
                { "Nickname", Nickname },
                { "GameID", GameID },
                { "Matrix", Matrix },
                { "TotalNormalReward", TotalNormalReward },
                { "Total", Total },
                { "TotalSpecialReward", TotalSpecialReward },
                { "Ip", Ip },
                { "AccountBalance", AccountBalance },
                { "AccountBagInfo", AccountBagInfo },
                { "ActionTime", DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local) },
                { "ExpireAt", DateTime.SpecifyKind(DateTime.Now.AddDays(60), DateTimeKind.Local) }};

            await trackingCollection.InsertOneAsync(document).ConfigureAwait(false);
        }

        public List<Tracking_SlotMachine_Detail_Log_Model> Get_SlotMachine_Detail_Jackpot_Log(int GameID, int page = 0, int PageRow = 100, long accountId = 0)
        {
            if (PageRow > 500)
                PageRow = 500;
            List<Tracking_SlotMachine_Detail_Log_Model> model = new List<Tracking_SlotMachine_Detail_Log_Model>();
            try
            {
                var database = _client.GetDatabase(NO_SQL_CONFIG.T09_SYSTEM_DATABASE_NAME);
                string collectionName = NO_SQL_CONFIG.Get_Slot_Jackpot_Detail_CollectionName(GameID);
                if (string.IsNullOrEmpty(collectionName))
                    return model;

                BsonDocument filter = new BsonDocument();
                if (accountId != 0)
                {
                    filter.Add("AccountID", accountId);
                }
                BsonDocument sort = new BsonDocument();
                sort.Add("ActionTime", -1.0);
                var options = new FindOptions<BsonDocument>()
                {
                    Skip = PageRow * page,
                    Sort = sort,
                    Limit = PageRow
                };
                var resultValue = database.GetCollection<BsonDocument>(collectionName).FindSync(filter, options).ToList();
                model = resultValue.Select(y => BsonSerializer.Deserialize<Tracking_SlotMachine_Detail_Log_Model>(y.ToBsonDocument())).ToList();

                return model;

                //IMongoCollection<Tracking_SlotMachine_Detail_Log_Model> trackingCollection = database.GetCollection<Tracking_SlotMachine_Detail_Log_Model>(collectionName);

                //if (accountId != 0)
                //{
                //    model  = trackingCollection.AsQueryable<Tracking_SlotMachine_Detail_Log_Model>().Where(x => x.AccountID == accountId).Skip(PageRow * page).Take(PageRow).OrderByDescending(y => y.ActionTime).ToList();
                //}
                //else
                //{
                //    model = trackingCollection.AsQueryable<Tracking_SlotMachine_Detail_Log_Model>().Where(x => x.AccountID > 0).Skip(PageRow * page).Take(PageRow).OrderByDescending(y => y.ActionTime).ToList();
                //}
                //return model;
            }
            catch (Exception exception)
            {
                return model;
            }
        }
        #endregion

        #region LOG BUY
        public async Task Tracking_Game_Event_Exchange_Item(long AccountID, long transId, long packageId, long quantity, bool isSuccess, string description)
        {

            var database = _client.GetDatabase(NO_SQL_CONFIG.T09_SYSTEM_DATABASE_NAME);
            string collectionName = NO_SQL_CONFIG.SLOTMACHINE_TUTIEN_SHOP_EXCHANGE_HISTORY_COLLECTION;

            IMongoCollection<BsonDocument> trackingCollection = database.GetCollection<BsonDocument>(collectionName);

            var indexBuilder = Builders<BsonDocument>.IndexKeys;
            var key = indexBuilder.Ascending("ExpireAt");
            var options = new CreateIndexOptions
            {
                ExpireAfter = new TimeSpan(0),
                Name = "ExpireAtIndex",

            };
            await trackingCollection.Indexes.CreateOneAsync(key, options).ConfigureAwait(false);

            BsonDocument document = new BsonDocument {
                { "AccountID", AccountID },
                { "TransactionId", transId },
                { "PackageId", packageId },
                { "Quantity", quantity },
                { "Description", description },
                { "IsSuccess", isSuccess },
                { "ActionTime", DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local) },
                { "ExpireAt", DateTime.SpecifyKind(DateTime.Now.AddDays(60), DateTimeKind.Local) }};

            await trackingCollection.InsertOneAsync(document).ConfigureAwait(false);
        }

        public List<Tracking_Game_Event_Exchange_Item_Log_Model> Get_Game_Event_Exchange_Item(long accountId = 0, int page = 0, int PageRow = 100)
        {
            if (PageRow > 500)
                PageRow = 500;
            List<Tracking_Game_Event_Exchange_Item_Log_Model> model = new List<Tracking_Game_Event_Exchange_Item_Log_Model>();
            try
            {
                var database = _client.GetDatabase(NO_SQL_CONFIG.T09_SYSTEM_DATABASE_NAME);
                IMongoCollection<Tracking_Game_Event_Exchange_Item_Log_Model> trackingCollection = database.GetCollection<Tracking_Game_Event_Exchange_Item_Log_Model>(NO_SQL_CONFIG.SLOTMACHINE_TUTIEN_SHOP_EXCHANGE_HISTORY_COLLECTION);

                if (accountId != 0)
                {
                    model = trackingCollection.AsQueryable<Tracking_Game_Event_Exchange_Item_Log_Model>().Where(x => x.AccountID == accountId).Skip(PageRow * page).Take(PageRow).OrderByDescending(y => y.ActionTime).ToList();
                }
                else
                {
                    model = trackingCollection.AsQueryable<Tracking_Game_Event_Exchange_Item_Log_Model>().Where(x => x.AccountID > 0).Skip(PageRow * page).Take(PageRow).OrderByDescending(y => y.ActionTime).ToList();
                }
                return model;
            }
            catch (Exception)
            {
                return model;
            }
        }

        public async Task Tracking_SlotMachine_SwapCat(long AccountID, string Username)
        {
            var database = _client.GetDatabase(NO_SQL_CONFIG.T09_SYSTEM_DATABASE_NAME);

            string collectionName = NO_SQL_CONFIG.SLOTMACHINE_PUNCH_CAT_SWAPCAT;
            if (string.IsNullOrEmpty(collectionName))
                return;
            IMongoCollection<BsonDocument> trackingCollection = database.GetCollection<BsonDocument>(collectionName);

            var indexBuilder = Builders<BsonDocument>.IndexKeys;
            var key = indexBuilder.Ascending("ExpireAt");
            var options = new CreateIndexOptions
            {
                ExpireAfter = new TimeSpan(0),
                Name = "ExpireAtIndex",

            };
            await trackingCollection.Indexes.CreateOneAsync(key, options).ConfigureAwait(false);

            BsonDocument document = new BsonDocument {
                { "AccountID",AccountID },
                { "Username", Username },
                { "ActionTime", DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local) },
                { "ExpireAt", DateTime.SpecifyKind(DateTime.Now.AddDays(60), DateTimeKind.Local) }};

            await trackingCollection.InsertOneAsync(document).ConfigureAwait(false);
        }

        public void Dispose()
        {
            _client.Cluster.Dispose();
        }
        #endregion

        public static string GetToDay_Totring(DateTime dateTime)
        {
            string tempDayString = String.Format("{0:d_M_yyyy}", dateTime);
            return tempDayString;
        }
    }
}
