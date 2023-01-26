using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoggerService.Model
{
    public class LogMongoModel
    {
        [BsonId]
        [BsonElement("Id")]
        [JsonIgnore]
        public ObjectId Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Status { get; set; }
        public DateTime ActionTime { get; set; }
        [JsonIgnore]
        public DateTime ExpireAt { get; set; }

    }

}
