using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
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
}
