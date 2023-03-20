using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.Request
{
    public class RequestGetByTime
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
