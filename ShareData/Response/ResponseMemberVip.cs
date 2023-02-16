using ShareData.DB.Member;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.Response
{
    public class ResponseMemberVip
    {
        public long CurrentPoint { get; set; }
        public long CurrentVip { get; set; }
        public string VipName { get; set; }
    }
}
