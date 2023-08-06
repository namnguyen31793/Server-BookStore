using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoggerService.Model
{
    public class Tracking_Ccu_Model
    {
        [JsonIgnore]
        [BsonId]
        [BsonElement("Id")]
        public ObjectId Id { get; set; }
        public long Ccu { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ActionTime { get; set; }
        [JsonIgnore]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ExpireAt { get; set; }
    }
}
