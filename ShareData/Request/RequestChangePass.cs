using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.Request
{
    public class RequestChangePass
    {
        public string OldPass { get; set; }
        public string NewPass { get; set; }
    }
    public class RequestForgot
    {
        public string mail { get; set; }
    }
}
