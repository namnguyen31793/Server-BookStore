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
}
