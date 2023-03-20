using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.DB.Users
{
    public class AccountVourcherModel
    {
        public int VourcherId { get; set; }
        public string VourcherName { get; set; }
        public int CountUse { get; set; }
        public int VourcherType { get; set; }
        public string VourcherReward { get; set; }
        public DateTime TTL { get; set; }
    }
}
