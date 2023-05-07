using BookStoreCMS.ModelMongo;
using LoggerService;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreCMS.Utils
{
    public class TrackingLogSystemInstance
    {
        private static object _syncObject = new object();
        private static TrackingLogSystemInstance _inst { get; set; }
        private static MongoClient _client;
        public static TrackingLogSystemInstance Inst
        {
            get
            {
                if (_inst == null)
                    lock (_syncObject)
                    {
                        if (_inst == null) _inst = new TrackingLogSystemInstance();
                        MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(NO_SQL_CONFIG.HOST_CONFIG));
                        settings.SslSettings = new SslSettings() { EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 };
                        settings.MaxConnectionIdleTime = TimeSpan.FromSeconds(30);
                        _client = new MongoClient(settings);
                    }
                return _inst;
            }
        }

        public List<Tracking_Action_Model> Get_ActionHome(string action, DateTime start, DateTime end)
        {
            List<Tracking_Action_Model> actionList = new List<Tracking_Action_Model>();
            var database = _client.GetDatabase(NO_SQL_CONFIG.API_TRACKING_SYSTEM_DATABASE_NAME);
            IMongoCollection<BsonDocument> trackingCollectionBson = database.GetCollection<BsonDocument>(NO_SQL_CONFIG.API_LOG_TRACKING_ACTION_HOME);

            DateTime timeCheckStart = end;
            while (timeCheckStart >= start)
            {
                var pipeline = new[]{new BsonDocument(){
                {"$match", new BsonDocument(){
                    {"Action", new BsonRegularExpression("^"+action+"$", "is")},
                    {"ActionTime", new BsonRegularExpression("^"+timeCheckStart.ToString("MM/dd/yyyy")+"$", "is")}
                    }}
                },new BsonDocument(){
                    {"$group", new BsonDocument(){
                        {"_id", BsonNull.Value},
                        {"CountAction", new BsonDocument(){
                            {"$sum", "$Count"}
                        }},
                        {"CountAccount", new BsonDocument(){
                            {"$addToSet", "$AccountId"}
                        }}
                    }}
                },new BsonDocument(){
                    {"$project", new BsonDocument(){
                        {"CountAction", "$CountAction"},
                        {"CountAccount", new BsonDocument(){
                            {"$size", "$CountAccount"}
                        }}
                    }}
                }};

                var results = trackingCollectionBson.Aggregate<BsonDocument>(pipeline).ToList();

                foreach (var document in results)
                {
                    var modelMongo = BsonSerializer.Deserialize<Tracking_Action_Mongo_Model>(document.ToBsonDocument());
                    if (modelMongo != null)
                        actionList.Add(new Tracking_Action_Model(modelMongo, timeCheckStart.ToString("MM/dd/yyyy")));
                }
                timeCheckStart = timeCheckStart.AddDays(-1);
            }
            return actionList;
        }

        public List<Tracking_Action_Model> Get_ActionUser(string action, DateTime start, DateTime end)
        {
                List<Tracking_Action_Model> actionList = new List<Tracking_Action_Model>();
                var database = _client.GetDatabase(NO_SQL_CONFIG.API_TRACKING_SYSTEM_DATABASE_NAME);
                IMongoCollection<BsonDocument> trackingCollectionBson = database.GetCollection<BsonDocument>(NO_SQL_CONFIG.API_LOG_TRACKING_ACTION_USER);
                DateTime timeCheckStart = end;
                while (timeCheckStart >= start)
                {
                    var pipeline = new[]{new BsonDocument(){
                        {"$match", new BsonDocument(){
                            {"Action", new BsonRegularExpression("^"+action+"$", "is")},
                            {"ActionTime", new BsonRegularExpression("^"+timeCheckStart.ToString("MM/dd/yyyy")+"$", "is")}
                            }}
                        },new BsonDocument(){
                            {"$group", new BsonDocument(){
                                {"_id", BsonNull.Value},
                                {"CountAction", new BsonDocument(){
                                    {"$sum", "$Count"}
                                }},
                                {"CountAccount", new BsonDocument(){
                                    {"$addToSet", "$AccountId"}
                                }}
                            }}
                        },new BsonDocument(){
                            {"$project", new BsonDocument(){
                                {"CountAction", "$CountAction"},
                                {"CountAccount", new BsonDocument(){
                                    {"$size", "$CountAccount"}
                                }}
                            }}
                        }};

                    var results = trackingCollectionBson.Aggregate<BsonDocument>(pipeline).ToList();

                    foreach (var document in results)
                    {
                        var modelMongo = BsonSerializer.Deserialize<Tracking_Action_Mongo_Model>(document.ToBsonDocument());
                        if (modelMongo != null)
                            actionList.Add(new Tracking_Action_Model(modelMongo, timeCheckStart.ToString("MM/dd/yyyy")));
                    }
                    timeCheckStart = timeCheckStart.AddDays(-1);
                }
                return actionList;
        }

        public List<Tracking_Action_Model> Get_ActionFindBook(string action, DateTime start, DateTime end)
        {
            List<Tracking_Action_Model> actionList = new List<Tracking_Action_Model>();
            var database = _client.GetDatabase(NO_SQL_CONFIG.API_TRACKING_SYSTEM_DATABASE_NAME);
            IMongoCollection<BsonDocument> trackingCollectionBson = database.GetCollection<BsonDocument>(NO_SQL_CONFIG.API_LOG_TRACKING_FIND_BOOK);

            DateTime timeCheckStart = end;
            while (timeCheckStart >= start)
            {
                var pipeline = new[]{new BsonDocument(){
                {"$match", new BsonDocument(){
                    {"Action", new BsonRegularExpression("^"+action+"$", "is")},
                    {"ActionTime", new BsonRegularExpression("^"+timeCheckStart.ToString("MM/dd/yyyy")+"$", "is")}
                    }}
                },new BsonDocument(){
                    {"$group", new BsonDocument(){
                        {"_id", BsonNull.Value},
                        {"CountAction", new BsonDocument(){
                            {"$sum", "$Count"}
                        }},
                        {"CountAccount", new BsonDocument(){
                            {"$addToSet", "$AccountId"}
                        }}
                    }}
                },new BsonDocument(){
                    {"$project", new BsonDocument(){
                        {"CountAction", "$CountAction"},
                        {"CountAccount", new BsonDocument(){
                            {"$size", "$CountAccount"}
                        }}
                    }}
                }};

                var results = trackingCollectionBson.Aggregate<BsonDocument>(pipeline).ToList();

                foreach (var document in results)
                {
                    var modelMongo = BsonSerializer.Deserialize<Tracking_Action_Mongo_Model>(document.ToBsonDocument());
                    if (modelMongo != null)
                        actionList.Add(new Tracking_Action_Model(modelMongo, timeCheckStart.ToString("MM/dd/yyyy")));
                }
                timeCheckStart = timeCheckStart.AddDays(-1);
            }
            return actionList;
        }
    }
}
