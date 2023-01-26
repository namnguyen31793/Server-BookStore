using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace LoggerService.Model
{
    public class Tracking_Event_Store_Buy_GiftCode_Log_Model
    {
        [BsonId]
        [BsonElement("Id")]
        public ObjectId Id { get; set; }
        public long AccountID { get; set; }
        public string TransIds { get; set; }
        public DateTime ActionTime { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}
