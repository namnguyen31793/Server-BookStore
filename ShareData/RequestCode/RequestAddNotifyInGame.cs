using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.RequestCode
{
    public class RequestAddNotifyInGame
    {
        public long Id { get; set; }
        public int Type { get; set; } // 0-add, 1-update, 2- remove
        public string Content { get; set; }
        public bool Status { get; set; }
        public DateTime TimeExpires { get; set; }
    }
}
