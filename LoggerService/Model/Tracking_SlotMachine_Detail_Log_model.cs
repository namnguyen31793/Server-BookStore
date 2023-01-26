using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoggerService.Model
{
    public class Tracking_SlotMachine_Detail_Log_Model
    {
        [BsonId]
        [BsonElement("Id")]
        public ObjectId Id { get; set; }
        public int SpinID { get; set; }
        public int Bet { get; set; }
        public long AccountID { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public int GameID { get; set; }
        [JsonIgnore]
        public string Matrix { get; set; }
        public long TotalNormalReward { get; set; }
        public long Total { get; set; }
        public long TotalSpecialReward { get; set; }
        public string Ip { get; set; }
        public long AccountBalance { get; set; }
        public string AccountBagInfo { get; set; }
        public bool Jackpot { get; set; }
        public string ExtendMatrixDescription { get; set; }
        public DateTime ActionTime { get; set; }
        [JsonIgnore]
        public DateTime ExpireAt { get; set; }
    }
    public class Response_Detail_Log_Model
    {
        public int SpinID { get; set; }
        public int Bet { get; set; }
        public long AccountID { get; set; }
        public string Username { get; set; }
        public long AccountBalance { get; set; }
        public long Total { get; set; }
        public long TotalSpecialReward { get; set; }
        public bool Jackpot { get; set; }
        public string ExtendMatrixDescription { get; set; }
        public DateTime ActionTime { get; set; }
        public Response_Detail_Log_Model(Tracking_SlotMachine_Detail_Log_Model data)
        {
            SpinID = data.SpinID;
            Bet = data.Bet;
            AccountID = data.AccountID;
            Username = data.Username;
            AccountBalance = data.AccountBalance;
            if (!data.Jackpot)
                Total = data.TotalNormalReward;
            else
                Total = data.Total;
            TotalSpecialReward = data.TotalSpecialReward;
            ActionTime = data.ActionTime;
            Jackpot = data.Jackpot;
            ExtendMatrixDescription = data.ExtendMatrixDescription;
        }
    }
}
