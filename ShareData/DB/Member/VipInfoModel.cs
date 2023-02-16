using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.DB.Member
{
    public class VipInfoModel
    {
        public int VipId { get; set; }
        public string VipName { get; set; }
        public int PointRequest { get; set; }
        public int VourcherId { get; set; }
        [JsonIgnore]
        public string DescriptionReward { get; set; }
    }
}
