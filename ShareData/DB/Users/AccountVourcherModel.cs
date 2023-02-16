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
        [JsonIgnore]
        public DateTime TTL { get; set; }
    }
}
