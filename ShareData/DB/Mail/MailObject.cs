using Newtonsoft.Json;
using System;

namespace ShareData.DB.Mail
{
    public class MailObject
    {
        public long MailId { get; set; }
        public string SenderNickname { get; set; }
        public string MailHeader { get; set; }
        public string MailContent { get; set; }
        public DateTime SendTime { get; set; }
        public int IsReaded { get; set; }
        public long Money { get; set; }
        [JsonIgnore]
        public bool AcceptMoney { get; set; }
        public string RewardBonusDescription { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    public class NotifyMailCmsObject
    {
        public long MailId { get; set; }
        public string SenderNickname { get; set; }
        public string MailHeader { get; set; }
        public string MailContent { get; set; }
        public DateTime SendTime { get; set; }
        public string RewardDescription { get; set; }
        [JsonIgnore]
        public DateTime EndLiveTime { get; set; }
        [JsonIgnore]
        public bool Status { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    public class NotifyMailObject
    {
        public long MailId { get; set; }
        public long AccountId { get; set; }
        public string SenderNickname { get; set; }
        public string MailHeader { get; set; }
        public string MailContent { get; set; }
        public DateTime SendTime { get; set; }
        public int IsReaded { get; set; }   //0 chưa đọc - 1 đọc, 2 - xóa
        [JsonIgnore]
        public DateTime ReadedTime { get; set; }
        public string RewardDescription { get; set; }
        [JsonIgnore]
        public bool AcceptReward { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
