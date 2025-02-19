﻿using System;
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
    public class RequestUpdaterOrderInfoModel
    {
        public long CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMobile { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        public bool Defaut { get; set; }
    }
    public class RequestCreateOrderInfoModelCMS
    {
        public long AccountId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMobile { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        public bool Defaut { get; set; }
    }
}
