using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoggerService.Model
{
    public class Tracking_Game_Event_Exchange_Item_Log_Model
    {
        [BsonId]
        [BsonElement("Id")]
        public ObjectId Id { get; set; }
        public long AccountID { get; set; }
        public long TransactionId { get; set; }
        public long PackageId { get; set; }
        public long Quantity { get; set; }
        public bool IsSuccess { get; set; }
        public string Description { get; set; }
        public DateTime ActionTime { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}
