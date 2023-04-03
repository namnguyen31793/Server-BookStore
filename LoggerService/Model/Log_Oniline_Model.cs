using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoggerService.Model
{
    public class Log_Oniline_Model
    {
        public long TimePlay { get; set; }
        public long AccountId { get; set; }
    }

    public class Log_Oniline_Hours_Model
    {
        [BsonId]
        [BsonElement("Id")]
        public ObjectId Id { get; set; }
        public long TimeOnline { get; set; }
        public string TimePlay { get; set; }
    }
}
