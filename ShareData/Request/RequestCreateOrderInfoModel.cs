using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.Request
{
    public class RequestCreateOrderInfoModel
    {
        public string CustomerName { get; set; }
        public string CustomerMobile { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        public bool Defaut { get; set; }
    }
}
