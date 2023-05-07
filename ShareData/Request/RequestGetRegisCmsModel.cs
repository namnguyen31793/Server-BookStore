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
    public class RequestGetCountRegis
    {
        public int Os { get; set; }   // 0 ALl, 1 web, 2 IOS, 3 Android
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
    public class RequestGetAction
    {
        public string Action { get; set; } 
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
