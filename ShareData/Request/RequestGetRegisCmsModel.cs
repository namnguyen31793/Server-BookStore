using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.Request
{
    public class RequestGetRegisCmsModel
    {
        public int Type { get; set; }   // -1:All, 0:Normal, 1:FB, 2:GG
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class RequestGetMemberCmsModel
    {
        public int Vip { get; set; }   // 2:Titanium, 3:Gold , 4:Platinum
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
