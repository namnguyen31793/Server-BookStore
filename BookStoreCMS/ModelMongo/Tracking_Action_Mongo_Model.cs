using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreCMS.ModelMongo
{
    public class Tracking_Action_Mongo_Model
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public double CountAction { get; set; }
        public double CountAccount { get; set; }
    }
    public class Tracking_Action_Model
    {
        public string Time { get; set; }
        public long CountAction { get; set; }
        public long CountAccount { get; set; }
        public Tracking_Action_Model() { }
        public Tracking_Action_Model(Tracking_Action_Mongo_Model Model, string time) {
            this.Time = time;
            this.CountAccount = (long)Model.CountAccount;
            this.CountAction = (long)Model.CountAction;
        }
    }

    public class Tracking_Buy_Book
    {
        [JsonIgnore]
        [BsonId]
        public BsonDocument Id { get; set; } // Sử dụng BsonDocument cho trường _id là document

        [BsonElement("Barcode")] // Ánh xạ trường Barcode trong document
        public string Barcode { get; set; }

        [BsonElement("Count")] // Ánh xạ trường Count trong document
        public long Count { get; set; }
    }
}
