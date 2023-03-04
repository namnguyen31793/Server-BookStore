using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.Request
{
    public class RequestSendMailModel
    {
        public long Accountid { get; set; }
        public string SenderNickname { get; set; }
        public string MailHeader { get; set; }
        public string MailContent { get; set; }
        public long Money { get; set; }
        public string RewardBonusDescription { get; set; }
    }
}
