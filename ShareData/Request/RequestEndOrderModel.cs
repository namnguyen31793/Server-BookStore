using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.Request
{
    public class RequestEndOrderModel
    {
        public long OrderId { get; set; }
        public string PrivateDescription { get; set; }
        public int Status { get; set; } //3 - success, 4- fail
    }
}
